using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDoListPlus.Services;
using ToDoListPlus.Views;

namespace ToDoListPlus.ViewModels
{

    public class MainViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly IServiceProvider _serviceProvider;

        //View Models
        public ToDoListViewModel? ToDoListVM { get;  }
        public AuthorizationViewModel? AuthorizationVM { get; }

        private bool _isLightMode = false;

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

        public MainViewModel(ToDoListViewModel toDoListVM, AuthorizationViewModel? authorizationVM)
        {
            AuthorizationVM = authorizationVM;

            ToDoListVM = toDoListVM;

            OpenSettingsCommand = new AsyncRelayCommand(OpenSettingsWindow);
            NewTaskCommand = new AsyncRelayCommand(OpenNewTaskWindow);
        }

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
        public async Task OpenNewTaskWindow()
        {
            var newTaskView = new NewTaskView();
            newTaskView.DataContext = App.Services.GetRequiredService<NewTaskViewModel>();
            var isDiagOpen = DialogHost.IsDialogOpen("RootDialog");

            if (!isDiagOpen)
            {
                var result = await DialogHost.Show(newTaskView, "RootDialog");
            }
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
