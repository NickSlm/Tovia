using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows;
using ToDoListPlus.Services;
using ToDoListPlus.Services.oauth2;

namespace ToDoListPlus.ViewModels
{
    public class AuthorizationViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand AuthorizationCommand => _authorizationCommand;

        private readonly DelegateCommand _authorizationCommand;

        private string _accessToken;
        private Window _parentWindow;
        public string AccessToken
        {
            get => _accessToken;
            set
            {
                _accessToken = value;
                OnPropertyChanged(nameof(AccessToken));
            }
        }

        string[] scopes = new string[] { "user.read", "Calendars.ReadWrite" };

        public AuthorizationViewModel(Window parentWindow)
        {
            AccessToken = "asdf";
            _parentWindow = parentWindow;
            _authorizationCommand = new DelegateCommand(AuthorizationButtonClick, CanExecute);

        }

        private async void AuthorizationButtonClick(object commandParameter)
        {


            AuthenticationResult authResult = null;
            var app = AuthService.ClientApp;

            IAccount firstAccount = (await app.GetAccountsAsync()).FirstOrDefault();

            if (firstAccount == null)
            {
                firstAccount = PublicClientApplication.OperatingSystemAccount;
            }
            try
            {
                authResult = await app.AcquireTokenSilent(scopes, firstAccount).ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");
                try
                {
                    authResult = await app.AcquireTokenInteractive(scopes)
                        .WithAccount(firstAccount)
                        .WithParentActivityOrWindow(new WindowInteropHelper(_parentWindow).Handle)
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    AccessToken = $"Error: {msalex.Message}";
                }
            }
            catch (Exception ex)
            {
                AccessToken = $"Error: {ex.Message}";
                return;
            }

            if (authResult != null)
            {
                AccessToken = authResult.AccessToken;
            }
            
        }

        private bool CanExecute(object commandParameter)
        {
            return true;
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
