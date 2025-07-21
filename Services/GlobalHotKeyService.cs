using GlobalHotKey;
using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Windows.Input;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class GlobalHotKeyService: IDisposable
    {
        private readonly HotKeyManager _manager = new();
        private readonly SettingsService _settingsService;

        public event Action? OnOverlayHotKeyPressed;
        public event Action? OnNewTaskHotKeyPressed;
        public Dictionary<string, (Key Key, ModifierKeys ModifierKey)> _storedKeys = new();

        public GlobalHotKeyService(SettingsService settingsService)
        {
            _settingsService = settingsService;

            _manager.KeyPressed += HotKeyManagerPressed;

            ApplySettings();

        }

        private void ApplySettings()
        {
            var userSettings = _settingsService.userSettings;

            foreach (var (name, setting) in userSettings.Hotkeys)
            {
                _storedKeys[name] = (setting.MainKey, setting.ModifierKey);
                _manager.Register(setting.MainKey, setting.ModifierKey);
            }
        }

        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            foreach (var(name, (key, modifier)) in _storedKeys)
            {
                if (e.HotKey.Key == key && e.HotKey.Modifiers == modifier)
                {
                    HandleHotKey(name);
                }
            }
        }

        private void HandleHotKey(string hotkeyName)
        {
            switch (hotkeyName)
            {
                case "Overlay":
                    OnOverlayHotKeyPressed?.Invoke();
                    break;
                case "NewTask":
                    OnNewTaskHotKeyPressed?.Invoke();
                    break;
            }
        }

        public void RegisterHotKey(string name, Key key, ModifierKeys modifier)
        {
            _manager.Unregister(_storedKeys[name].Key, _storedKeys[name].ModifierKey);
            _storedKeys[name] = (key, modifier);
            _manager.Register(key, modifier);
        }

        public void Dispose() 
        {
            _manager.Dispose();
        }
    }
}
