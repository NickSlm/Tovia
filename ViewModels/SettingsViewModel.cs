using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDoListPlus.Models;
using ToDoListPlus.Services;
using ToDoListPlus.Views;

namespace ToDoListPlus.ViewModels
{
    public class SettingsViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly GlobalHotKeyService _globalHotKeyService;
        private readonly AppThemeService _appThemeService;
        private readonly OverlayViewModel _overlayViewModel;
        private readonly SettingsService _settingsService;

        private bool _isDarkTheme { get; set; }
        private OverlayPosition _overlayPos { get; set; } = OverlayPosition.TopLeft;

        public IRelayCommand UpdateThemeCommand { get;}
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                _isDarkTheme = value;
                OnPropertyChanged(nameof(IsDarkTheme));
            }
        }
        public Dictionary<string, (Key key, ModifierKeys modifier)> _hotkeySettings = new();
        public Dictionary<string, KeyStroke> _keyStrokes { get; } = new();
        public IRelayCommand SaveSettingsCommand { get; }
        public Array PositionOptions => Enum.GetValues(typeof(OverlayPosition));
        public OverlayPosition OverlayPos
        {
            get => _overlayPos;
            set
            {
                _overlayPos = value;
                OnPropertyChanged(nameof(OverlayPos));
            }
        }

        public SettingsViewModel( 
            GlobalHotKeyService globalHotKeyService, 
            AppThemeService appThemeService,
            OverlayViewModel overlayViewModel,
            SettingsService settingsService
            )
        {

            _settingsService = settingsService;
            _globalHotKeyService = globalHotKeyService;
            _appThemeService = appThemeService;
            _overlayViewModel = overlayViewModel;

            var conf = _settingsService.Load();
            var settings = conf.Get<UserSettings>();

            IsDarkTheme = settings.Theme.BaseTheme == "light" ? false : true;

            UpdateThemeCommand = new RelayCommand(() =>
            {
                _appThemeService.ChangeTheme(IsDarkTheme);
            });

            foreach (var (name, sett) in settings.Hotkeys)
            {
                _hotkeySettings[name] = (sett.MainKey, sett.ModifierKey);
                var keystroke = new KeyStroke
                {
                    keyStroke = $"{_hotkeySettings[name].modifier.ToString()} + {_hotkeySettings[name].key.ToString()}"
                };
                _keyStrokes[name] = keystroke;
            }
            SaveSettingsCommand = new RelayCommand(saveSettings);
        }

        public void saveSettings() 
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
                    },
                    ["NewTask"] = new HotkeySettings
                    {
                        MainKey = _hotkeySettings["NewTask"].key,
                        ModifierKey = _hotkeySettings["NewTask"].modifier
                    }
                },
                Window = new WindowSettings
                {
                    TopPos = TopPos,
                    LeftPos = LeftPos
                },
                Theme = new ThemeSettings
                {
                    BaseTheme = _isDarkTheme ? "dark" : "light"
                }
            };

            foreach (var (name, (key, modifier)) in _hotkeySettings)
            {
                _globalHotKeyService.RegisterHotKey(name, key, modifier);
            }

            _overlayViewModel.UpdatePosition(TopPos, LeftPos);
            _settingsService.Save(userSettings);
            DialogHost.Close("RootDialog");


        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
