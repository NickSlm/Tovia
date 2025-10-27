﻿using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using Tovia.Views;
using Tovia.Services;
using Tovia.interfaces;

namespace Tovia.ViewModels
{
    public class AuthorizationViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly IDialogService _dialogService;
        private readonly AppStateService _appStateService;
        private string _accountUsername;
        private BitmapImage _accountPhoto;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AuthorizationViewModel(AuthService authService, IDialogService dialogService, AppStateService appStateService)
        {
            _authService = authService;
            _dialogService = dialogService;
            _appStateService = appStateService;

            AuthorizationCommand = new AsyncRelayCommand(AuthorizationButtonClick);
            SignOutCommand = new AsyncRelayCommand(SignOutButtonClick);
            CloseCommand = new RelayCommand(CloseButtonClick);
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
        public BitmapImage AccountPhoto
        {
            get => _accountPhoto;
            set
            {
                _accountPhoto = value;
                OnPropertyChanged(nameof(AccountPhoto));
            }
        }

        public IAsyncRelayCommand SignOutCommand { get; }
        public IAsyncRelayCommand AuthorizationCommand { get; }
        public IRelayCommand CloseCommand { get; }

        private void CloseButtonClick()
        {
            var authWindow = Application.Current.Windows
                                 .OfType<AuthorizationWindow>()
                                 .FirstOrDefault();
            authWindow.DialogResult = false;
        }
        private async Task AuthorizationButtonClick()
        {
            await _authService.Authorize();
            AccountUsername = _authService.AccountUsername;
            AccountPhoto = _authService.AccountProfilePic;

            var authWindow = Application.Current.Windows.OfType<AuthorizationWindow>().FirstOrDefault();

            if (authWindow != null)
            {
                authWindow.DialogResult = true;
                _appStateService.SignIn();
            }
        }
        private async Task SignOutButtonClick()
        {
            await _authService.SignOutAsync();

            _appStateService.SignOut();

            AccountUsername = string.Empty;
            AccountPhoto = null;

            var result = _dialogService.ShowLoginDialog();
            if (!result == true)
            {
                Application.Current.Shutdown();
            }
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
