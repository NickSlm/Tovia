using System.Diagnostics;
using System.Windows;

namespace Tovia.Services
{
    public class AppStateService
    {
        public bool IsOnline = false;

        public event Action? UserLoggedIn;
        public event Action? UserLoggedOut;

        public void SignIn()
        {
            IsOnline = true;
            UserLoggedIn?.Invoke();
        }
        public void SignOut()
        {
            IsOnline = false;
            UserLoggedOut?.Invoke();
        }

    }
}
