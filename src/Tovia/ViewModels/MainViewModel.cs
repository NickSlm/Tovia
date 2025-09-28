using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows.Controls;
using Tovia;
using Tovia.Views;

namespace Tovia.ViewModels
{

    public class MainViewModel: INotifyPropertyChanged
    {
        private bool _isLightMode = false;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        public MainViewModel(ToDoListViewModel toDoListVM, AuthorizationViewModel? authorizationVM)
        {
            AuthorizationVM = authorizationVM;
            ToDoListVM = toDoListVM;

            OpenSettingsCommand = new AsyncRelayCommand(OpenSettingsWindow);
        }

        public ToDoListViewModel? ToDoListVM { get;  }
        public AuthorizationViewModel? AuthorizationVM { get; }
        public bool IsLightMode
        {
            get => _isLightMode;
            set
            {
                _isLightMode = value;
                OnPropertyChanged(nameof(IsLightMode));
            }
        }

        public IAsyncRelayCommand OpenSettingsCommand { get; }
        public IAsyncRelayCommand NewTaskCommand { get; }

        public async Task OpenSettingsWindow()
        {
            var settingsView = new SettingsView();
            settingsView.DataContext = App.Services.GetRequiredService<SettingsViewModel>();

            var isDiagOpen = DialogHost.IsDialogOpen("RootDialog");

            if (!isDiagOpen)
            {
                var result = await DialogHost.Show(settingsView, "RootDialog");

            }
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
