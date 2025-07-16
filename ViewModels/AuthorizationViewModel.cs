using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using ToDoListPlus.Services;
using ToDoListPlus.Views;

namespace ToDoListPlus.ViewModels
{
    public class AuthorizationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public IAsyncRelayCommand SignOutCommand { get; }
        public IAsyncRelayCommand AuthorizationCommand { get; }
        public IRelayCommand CloseCommand { get; }


        private readonly AuthService _authService;
        private readonly IDialogService _dialogService;
        private readonly AppStateService _appStateService;
        private string _accountUsername;



        public string AccountUsername
        {
            get => _accountUsername;
            set
            {
                _accountUsername = value;
                OnPropertyChanged(nameof(AccountUsername));
            }
        }

        public AuthorizationViewModel(AuthService authService, IDialogService dialogService, AppStateService appStateService)
        {
            _authService = authService;
            _dialogService = dialogService;
            _appStateService = appStateService;

            AuthorizationCommand = new AsyncRelayCommand(AuthorizationButtonClick);
            SignOutCommand = new AsyncRelayCommand(SignOutButtonClick);
            CloseCommand = new RelayCommand(CloseButtonClick);
        }

        private void CloseButtonClick()
        {
            var authWindow = Application.Current.Windows
                                .OfType<AuthorizationWindow>()
                                .FirstOrDefault();
            authWindow.DialogResult = false;
        }

        private async Task AuthorizationButtonClick()
        {
            var result = await _authService.Authorize();
            AccountUsername = _authService.AccountUsername;

            var authWindow = Application.Current.Windows.OfType<AuthorizationWindow>().FirstOrDefault();

            if (authWindow != null)
            {
                authWindow.DialogResult = true;
                _appStateService.SignIn();
            }
        }

        private async Task SignOutButtonClick()
        {
            string SignoutRes = await _authService.SignOutAsync();

            _appStateService.SignOut();

            AccountUsername = string.Empty;

            var result = _dialogService.ShowLoginDialog();
            if (!result == true)
            {
                Application.Current.Shutdown();
            }
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
