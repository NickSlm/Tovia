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
        public IRelayCommand ToggleThemeCommand { get; }


        public MainViewModel(ToDoListViewModel toDoListVM, AuthorizationViewModel? authorizationVM)
        {
            AuthorizationVM = authorizationVM;

            ToDoListVM = toDoListVM;

            OpenSettingsCommand = new AsyncRelayCommand(OpenSettingsWindow);
            NewTaskCommand = new AsyncRelayCommand(OpenNewTaskWindow);
            ToggleThemeCommand = new RelayCommand(ChangeTheme);
        }

        public async Task OpenSettingsWindow()
        {
            var settingsView = new SettingsView();
            settingsView.DataContext = App.Services.GetRequiredService<SettingsViewModel>();
            var result = await DialogHost.Show(settingsView, "RootDialog");
        }
        public async Task OpenNewTaskWindow()
        {
            var newTaskView = new NewTaskView();
            var newTaskViewModel = App.Services.GetRequiredService<NewTaskViewModel>();
            newTaskView.DataContext = newTaskViewModel;
            var isDiagOpen = DialogHost.IsDialogOpen("RootDialog");

            if (!isDiagOpen)
            {
                var result = await DialogHost.Show(newTaskView, "RootDialog");
            }
        }
        public void ChangeTheme()
        {
            PaletteHelper palette = new PaletteHelper();
            var theme = palette.GetTheme();
            theme.SetDarkTheme();
            palette.SetTheme(theme);
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
