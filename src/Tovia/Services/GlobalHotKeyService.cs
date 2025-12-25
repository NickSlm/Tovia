using GlobalHotKey;
using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Windows.Input;
using Tovia.Models;

namespace Tovia.Services
{
    public class GlobalHotKeyService: IDisposable
    {
        private readonly HotKeyManager _manager = new();
        private readonly IConfiguration _config;
        private readonly HotkeyCategory _hotkeySettings;
        public Dictionary<string, (Key Key, ModifierKeys ModifierKey)> _storedKeys = new();

        public event Action? OnOverlayHotKeyPressed;
        public event Action? OnNewTaskHotKeyPressed;

        public GlobalHotKeyService(IConfiguration config)
        {
            _config = config;
            _hotkeySettings = config.GetSection("Hotkeys").Get<HotkeyCategory>();
            
            _manager.KeyPressed += HotKeyManagerPressed;

            ApplySettings();

        }

        private void ApplySettings()
        {
            
            foreach (var category in _hotkeySettings)
            {
                _storedKeys[category.Key] = (category.Value.MainKey, category.Value.ModifierKey);
                _manager.Register(category.Value.MainKey, category.Value.ModifierKey);
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
