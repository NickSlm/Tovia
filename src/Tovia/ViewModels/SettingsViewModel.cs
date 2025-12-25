using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Tovia;
using Tovia.Views;
using Tovia.Models;
using Tovia.Services;

namespace Tovia.ViewModels
{
    public class SettingsViewModel: INotifyPropertyChanged
    {
        private readonly GlobalHotKeyService _globalHotKeyService;
        private readonly AppThemeService _appThemeService;
        private readonly OverlayViewModel _overlayViewModel;
        private readonly SettingsService _settingsService;
        private readonly IConfiguration _config;
        private bool _isDarkTheme;
        private string _inProgressTaskColor;
        private string _failedTaskColor;
        private string _completedTaskColor;
        private OverlayPosition _overlayPos = OverlayPosition.TopLeft;
        public Dictionary<string, (Key key, ModifierKeys modifier)> _hotkeySettings = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsViewModel( 
            GlobalHotKeyService globalHotKeyService, 
            AppThemeService appThemeService,
            OverlayViewModel overlayViewModel,
            SettingsService settingsService,
            IConfiguration config
            )
        {
            _settingsService = settingsService;
            _globalHotKeyService = globalHotKeyService;
            _appThemeService = appThemeService;
            _overlayViewModel = overlayViewModel;
            _config = config;
            _settingsService.SettingsChanged += (s, e) => ApplySettings();

            ApplySettings();

            UpdateThemeCommand = new RelayCommand(() =>
            {
                _appThemeService.ChangeTheme(IsDarkTheme);
            });
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        public IRelayCommand UpdateThemeCommand { get;}
        public IRelayCommand SaveSettingsCommand { get; }
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                _isDarkTheme = value;
                OnPropertyChanged(nameof(IsDarkTheme));
            }
        }
        public string InProgressTaskColor
        {
            get => _inProgressTaskColor;
            set
            {
                _inProgressTaskColor = value;
                OnPropertyChanged(nameof(InProgressTaskColor));
            }
        }
        public string FailedTaskColor
        {
            get => _failedTaskColor;
            set
            {
                _failedTaskColor = value;
                OnPropertyChanged(nameof(FailedTaskColor));
            }
        }
        public string CompletedTaskColor
        {
            get => _completedTaskColor;
            set
            {
                _completedTaskColor = value;
                OnPropertyChanged(nameof(CompletedTaskColor));
            }
        }
        public OverlayPosition OverlayPos
        {
            get => _overlayPos;
            set
            {
                _overlayPos = value;
                OnPropertyChanged(nameof(OverlayPos));
            }
        }
        public Dictionary<string, KeyStroke> _keyStrokes { get; } = new();
        public Array PositionOptions => Enum.GetValues(typeof(OverlayPosition));


        public void ApplySettings()
        {
            var appearance = _config.GetSection("Appearance").Get<AppearanceSettings>();
            var hotkeys = _config.GetSection("Hotkeys").Get<HotkeyCategory>();

            IsDarkTheme = appearance.BaseTheme == "light" ? false : true;

            InProgressTaskColor = appearance.InProgressTask;
            FailedTaskColor = appearance.FailedTask;
            CompletedTaskColor = appearance.CompleteTask;


            foreach (var category in hotkeys)
            {
                _hotkeySettings[category.Key] = (category.Value.MainKey, category.Value.ModifierKey);
                var keystroke = new KeyStroke
                {
                    keyStroke = $"{category.Value.ModifierKey} + {category.Value.MainKey}"
                };
                _keyStrokes[category.Key] = keystroke;
            }
        }
        public void SaveSettings() 
        {
            var overlayWindow = App.Services.GetRequiredService<OverlayWindow>();
            var windowWidth = overlayWindow.Width;
            var windowHeight = overlayWindow.Height;

            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            double TopPos = 0;
            double LeftPos = 0;

            switch (OverlayPos)
            {
                case OverlayPosition.TopLeft:
                    TopPos = 0;
                    LeftPos = 0;
                    break;
                case OverlayPosition.TopRight:
                    TopPos = 0;
                    LeftPos = screenWidth - windowWidth;
                    break;
                case OverlayPosition.BottomLeft:
                    TopPos = screenHeight - windowHeight;
                    LeftPos = 0;
                    break;
                case OverlayPosition.BottomRight:
                    TopPos = screenHeight - windowHeight;
                    LeftPos = screenWidth - windowWidth;
                    break;
            }

            UserSettings userSettings = new UserSettings
            {
                Hotkeys = new Dictionary<string, HotkeySettings>
                {
                    ["Overlay"] = new HotkeySettings
                    {
                        MainKey = _hotkeySettings["Overlay"].key,
                        ModifierKey = _hotkeySettings["Overlay"].modifier
                    }
                },
                Window = new WindowSettings
                {
                    TopPos = TopPos,
                    LeftPos = LeftPos
                },
                Appearance = new AppearanceSettings
                {
                    BaseTheme = _isDarkTheme ? "dark" : "light",
                    FailedTask = FailedTaskColor,
                    InProgressTask = InProgressTaskColor,
                    CompleteTask = CompletedTaskColor
                }
            };

            foreach (var (name, (key, modifier)) in _hotkeySettings)
            {
                _globalHotKeyService.RegisterHotKey(name, key, modifier);
            }
            _settingsService.Save(userSettings);
            _overlayViewModel.UpdatePosition(TopPos, LeftPos);

            DialogHost.Close("RootDialog");
        }
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
