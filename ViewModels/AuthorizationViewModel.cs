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

        private string _accountUsername;
        private Visibility _signinIsVisible;
        private Visibility _signoutIsVisible;
        private Window _parentWindow;

        public Visibility SignoutIsVisible
        {
            get => _signoutIsVisible;
            set
            {
                _signoutIsVisible = value;
                OnPropertyChanged(nameof(SignoutIsVisible));
            }
        }

        public Visibility SigninIsVisible
        {
            get => _signinIsVisible;
            set
            {
                _signinIsVisible = value;
                OnPropertyChanged(nameof(SigninIsVisible));
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

        public AuthorizationViewModel(AuthService authService)
        {
            _authService = authService;
            _signOutCommand = new DelegateCommand(SignOutButtonClick, CanExecute);
            _authorizationCommand = new DelegateCommand(AuthorizationButtonClick, CanExecute);

            SigninIsVisible = Visibility.Visible;
            SignoutIsVisible = Visibility.Collapsed;
        }

        public void SetParentWindow(Window parentWindow)
        {
            _parentWindow = parentWindow;
        }

        public void CloseParentWindow()
        {
            _parentWindow?.Close();
        }

        private async void AuthorizationButtonClick(object commandParameter)
        {
            AccountUsername = await _authService.GetAccessTokenAsync(_parentWindow);
            if (!string.IsNullOrEmpty(AccountUsername))
            {
                SigninIsVisible = Visibility.Collapsed;
                SignoutIsVisible = Visibility.Visible;
            }
        }

        private async void SignOutButtonClick(object commandParamater)
        {
            await _authService.SignOutAsync();
            AccountUsername = string.Empty;
            SigninIsVisible = Visibility.Visible;
            SignoutIsVisible = Visibility.Collapsed;
        }

        private bool CanExecute(object commandParameter)
        {
            return true;
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
