using GlobalHotKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToDoListPlus.Services
{
    public class GlobalHotKeyService: IDisposable
    {
        private readonly HotKeyManager _manager = new();

        public event Action? OnOverlayHotKeyPressed;

        public GlobalHotKeyService() 
        {
            _manager.Register(Key.Space, ModifierKeys.Control);
            _manager.KeyPressed += HotKeyManagerPressed;
        }

        private void HotKeyManagerPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.HotKey.Key == Key.Space && e.HotKey.Modifiers == ModifierKeys.Control)
            {
                OnOverlayHotKeyPressed?.Invoke();
            }

        }
        public void Dispose() 
        {
            _manager.Dispose();
        }
    }
}
