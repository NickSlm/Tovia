using System.Diagnostics;
using System.Windows;
using Tovia.interfaces;
using Tovia.Models;

namespace Tovia.Services
{
    public class AppStateService
    {

        public IAuthProvider? AuthProvider;
        public string? AccessToken;
        public event Action? UserLoggedIn;
        public event Action? UserLoggedOut;
        public UserProfile? User { get; set; }

        public async Task SignIn(IAuthProvider authProvider)
        {
            AuthProvider = authProvider;

            AccessToken = await AuthProvider.SignInAsync();
            User = await AuthProvider.LoadProfileAsync();
            UserLoggedIn?.Invoke();
        }
        public async Task SignOut()
        {
            await AuthProvider.SignOutAsync();
            AuthProvider = null;
            UserLoggedOut?.Invoke();
        }
    }
}
