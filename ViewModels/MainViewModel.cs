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

        //View Models
        public ToDoListViewModel? ToDoListVM { get;  }
        public AuthorizationViewModel? AuthorizationVM { get; }

        public ICommand OpenSettingsCommand => _openSettingsCommand;
        public ICommand NewTaskCommand => _newTaskCommand;

        private readonly IServiceProvider _serviceProvider;
        private readonly DelegateCommand _openSettingsCommand;
        private readonly DelegateCommand _newTaskCommand;



        public MainViewModel(ToDoListViewModel toDoListVM, AuthorizationViewModel? authorizationVM)
        {
            AuthorizationVM = authorizationVM;

            ToDoListVM = toDoListVM;

            _openSettingsCommand = new DelegateCommand(OpenSettingsWindow, CanExecute);
            _newTaskCommand = new DelegateCommand(OpenNewTaskWindow, CanExecute);
        }

        private bool CanExecute(object commandParameter)
        {
            return true;
        }

        public async void OpenSettingsWindow(object commandParameter)
        {
            var settingsView = new SettingsView();
            settingsView.DataContext = App.Services.GetRequiredService<SettingsViewModel>();
            var result = await DialogHost.Show(settingsView, "RootDialog");
        }

        public async void OpenNewTaskWindow(object commandParameter)
        {
            var newTaskView = new NewTaskView();
            newTaskView.DataContext = App.Services.GetRequiredService<NewTaskViewModel>();
            var result = await DialogHost.Show(newTaskView, "RootDialog");
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
