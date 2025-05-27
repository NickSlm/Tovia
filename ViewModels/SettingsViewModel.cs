using Microsoft.Extensions.DependencyInjection;
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

        private HotkeySettings _overlayHotkeySettings;
        private readonly DelegateCommand _saveSettingsCommand;
        private readonly GlobalHotKeyService _globalHotKeyService;
        private readonly OverlayViewModel _overlayViewModel;

        public Dictionary<string, (Key key, ModifierKeys modifier)> _hotkeySettings = new();
        public Dictionary<string, KeyStroke> _keyStrokes { get; } = new();

        private OverlayPosition _overlayPos { get; set; } = OverlayPosition.TopLeft;
        public ICommand SaveSettingsCommand => _saveSettingsCommand;
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

        public SettingsViewModel(IOptions<Dictionary<string, HotkeySettings>> hotkeyOptions, GlobalHotKeyService globalHotKeyService, OverlayViewModel overlayViewModel)
        {
            _globalHotKeyService = globalHotKeyService;
            _overlayViewModel = overlayViewModel;

            foreach (var (name, setting) in hotkeyOptions.Value)
            {
                _hotkeySettings[name] = (setting.MainKey, setting.ModifierKey);

                var keystroke = new KeyStroke
                {
                    keyStroke = $"{_hotkeySettings[name].modifier.ToString()} + {_hotkeySettings[name].key.ToString()}"
                };
                _keyStrokes[name] = keystroke;

            }
            _saveSettingsCommand = new DelegateCommand(saveSettings, canExecute);
        }

        public void saveSettings(object commandParameter) 
        {

            //Update settings in real time
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
                }
            };

            foreach (var (name, (key, modifier)) in _hotkeySettings)
            {
                _globalHotKeyService.RegisterHotKey(name, key, modifier);
            }
            _overlayViewModel.UpdatePosition(TopPos, LeftPos);
            SettingsWriter.UpdateSettings(userSettings);
        }

        private bool canExecute(object commandParameter) { return true; }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
