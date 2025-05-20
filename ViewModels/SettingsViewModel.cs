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

        private HotkeySettings _hotkeySettings;
        private readonly DelegateCommand _saveSettingsCommand;
        private string _keyStroke { get; set; }
        private Key _mainKey { get; set; }
        private ModifierKeys _modifierKey { get; set; }
        private OverlayPosition _overlayPos { get; set; } = OverlayPosition.TopLeft;


        public ICommand SaveSettingsCommand => _saveSettingsCommand;
        public Array PositionOptions => Enum.GetValues(typeof(OverlayPosition));
        public Key MainKey
        {
            get => _mainKey;
            set
            {
                _mainKey = value;
                OnPropertyChanged(nameof(MainKey));
            }
        }
        public ModifierKeys ModifierKey
        {
            get => _modifierKey;
            set
            {
                _modifierKey = value;
                OnPropertyChanged(nameof(ModifierKey));
            }
        }
        public string KeyStroke
        {
            get => _keyStroke;
            set
            {
                _keyStroke = value;
                OnPropertyChanged(nameof(KeyStroke));
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

        public SettingsViewModel(IOptions<HotkeySettings> options)
        {
            _hotkeySettings = options.Value;

            MainKey = _hotkeySettings.MainKey;
            ModifierKey = _hotkeySettings.ModifierKey;
            KeyStroke = $"{_hotkeySettings.ModifierKey.ToString()} + {_hotkeySettings.MainKey.ToString()}";

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
                Hotkeys = new HotkeySettings
                {
                    MainKey = MainKey,
                    ModifierKey = ModifierKey
                },
                Window = new WindowSettings
                {
                    TopPos = TopPos,
                    LeftPos = LeftPos
                }
            };

            var globalHotKeyService = App.Services.GetRequiredService<GlobalHotKeyService>();
            var overlayViewModel = App.Services.GetRequiredService<OverlayViewModel>();

            globalHotKeyService.RegisterHotKey(_hotkeySettings.MainKey, _hotkeySettings.ModifierKey);
            overlayViewModel.UpdatePosition(TopPos, LeftPos);
            SettingsWriter.UpdateSettings(userSettings);
        }

        private bool canExecute(object commandParameter) { return true; }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
