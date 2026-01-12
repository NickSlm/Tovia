using System.Diagnostics;
using System.Windows;
using Tovia.interfaces;
using Tovia.Models;

namespace Tovia.Services
{
    public class AppStateService
    {


        public event Action? UserLoggedIn;
        public event Action? UserLoggedOut;

        public AuthSession AuthSession { get; private set; }
        public ITaskProvider TaskProvider { get; private set; }
        public IAuthProvider AuthProvider { get; private set; }

        public async Task SignIn(IAuthProvider authProvider)
        {
            AuthProvider = authProvider;
            AuthSession = await AuthProvider.SignInAsync();
            TaskProvider = AuthSession.TaskProvider;
            UserLoggedIn?.Invoke();
        }
        public async Task SignOut()
        {
            await AuthProvider.SignOutAsync();
            AuthProvider = null;
            AuthSession = null;
            TaskProvider = null;
            UserLoggedOut?.Invoke();
        }
    }
}
