using GlobalHotKey;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class GlobalHotKeyService: IDisposable
    {
        private readonly HotKeyManager _manager = new();

        public Key _storedKey;
        public ModifierKeys _storedModifier;


        public event Action? OnOverlayHotKeyPressed;

        public GlobalHotKeyService(IOptions<HotkeySettings> options) 
        {
            _manager.KeyPressed += HotKeyManagerPressed;
            _storedKey = options.Value.MainKey;
            _storedModifier = options.Value.ModifierKey;
            _manager.Register(_storedKey, _storedModifier);
        }

        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == _storedKey && e.HotKey.Modifiers == _storedModifier)
            {
                OnOverlayHotKeyPressed?.Invoke();
            }
        }

        public void RegisterHotKey(Key key, ModifierKeys modifier)
        {
            _manager.Unregister(_storedKey, _storedModifier);

            _storedKey = key;
            _storedModifier = modifier;

            _manager.Register(key, modifier);
        }
        public void Dispose() 
        {
            _manager.Dispose();
        }
    }
}
