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
using ToDoListPlus.Views;

namespace ToDoListPlus.ViewModels
{
    public class AuthorizationViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand AuthorizationCommand => _authorizationCommand;
        public ICommand SignOutCommand => _signOutCommand;


        private readonly DelegateCommand _authorizationCommand;
        private readonly DelegateCommand _signOutCommand;
        private readonly AuthService _authService;
        private readonly IDialogService _dialogService;
        private readonly IAppStateResetService _appStateResetService;

        private string _accountUsername;
        private bool _eventIsEnabled = false;
        private bool _isSignedIn = false;


        public bool IsSignedIn
        {
            get => _isSignedIn;
            set
            {
                _isSignedIn = value;
                OnPropertyChanged(nameof(IsSignedIn));
            }
        }
        public bool EventIsEnabled
        {
            get => _eventIsEnabled;
            set
            {
                _eventIsEnabled = value;
                OnPropertyChanged(nameof(EventIsEnabled));
            }
        }
        public string AccountUsername
        {
            get => _accountUsername;
            set
            {
                _accountUsername = value;
                OnPropertyChanged(nameof(AccountUsername));
            }
        }

        public AuthorizationViewModel(AuthService authService, IDialogService dialogService, IAppStateResetService resetService)
        {
            _appStateResetService = resetService;
            _dialogService = dialogService;
            _authService = authService;
            _signOutCommand = new DelegateCommand(SignOutButtonClick, CanExecute);
            _authorizationCommand = new DelegateCommand(AuthorizationButtonClick, CanExecute);

        }


        private async void AuthorizationButtonClick(object commandParameter)
        {
            await _authService.GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(_authService.AccessToken))
            {
                AccountUsername = _authService.AccountUsername;
                IsSignedIn = true;
                EventIsEnabled = true;


                Application.Current.Windows
                .OfType<AuthorizationWindow>()
                .FirstOrDefault()!
                .DialogResult = true;
            }
        }

        private async void SignOutButtonClick(object commandParamater)
        {
            string SignoutRes = await _authService.SignOutAsync();

            _appStateResetService.ResetState();
            AccountUsername = string.Empty;
            IsSignedIn = false;
            EventIsEnabled = false;

            var result = _dialogService.ShowLoginDialog();
            if (!result == true)
            {
                Application.Current.Shutdown();
            }

        }

        private bool CanExecute(object commandParameter)
        {
            return true;
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
