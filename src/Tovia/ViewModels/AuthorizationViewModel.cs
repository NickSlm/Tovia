using CommunityToolkit.Mvvm.Input;
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
        private readonly MicrosoftAuthService _microsoftAuth;
        private readonly GoogleAuthService _googleAuth;
        private readonly IDialogService _dialogService;
        private readonly AppStateService _appStateService;
        private bool _isSyncing;
        private string _accountUsername;
        private BitmapImage _accountPhoto;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AuthorizationViewModel(MicrosoftAuthService microsoftAuth, GoogleAuthService googleAuth , IDialogService dialogService, AppStateService appStateService)
        {
            _microsoftAuth = microsoftAuth;
            _googleAuth = googleAuth;
            _dialogService = dialogService;
            _appStateService = appStateService;

            AuthorizationCommand = new AsyncRelayCommand(AuthorizationButtonClick);
            GoogleAuthCommand = new AsyncRelayCommand(GoogleAuthorization);
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

        public bool IsSyncing
        {
            get => _isSyncing;
            set
            {
                _isSyncing = value;
                OnPropertyChanged(nameof(IsSyncing));
            }
        }

        public IAsyncRelayCommand SignOutCommand { get; }
        public IAsyncRelayCommand GoogleAuthCommand { get; }
        public IAsyncRelayCommand AuthorizationCommand { get; }
        public IRelayCommand CloseCommand { get; }

        private void CloseButtonClick()
        {
            var authWindow = Application.Current.Windows
                                 .OfType<AuthorizationWindow>()
                                 .FirstOrDefault();
            authWindow.DialogResult = false;
        }
        private async Task GoogleAuthorization()
        {
            await _googleAuth.Authorize();
        }
        private async Task AuthorizationButtonClick()
        {
            await _microsoftAuth.Authorize();
            AccountUsername = _microsoftAuth.AccountUsername;
            AccountPhoto = _microsoftAuth.AccountProfilePic;

            _appStateService.SignIn();

            var authWindow = Application.Current.Windows.OfType<AuthorizationWindow>().FirstOrDefault();

            if (authWindow != null)
            {
                authWindow.DialogResult = true;
            }
        }
        private async Task SignOutButtonClick()
        {
            await _microsoftAuth.SignOutAsync();

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
