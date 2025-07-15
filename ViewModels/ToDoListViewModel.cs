using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using ToDoListPlus.Models;
using ToDoListPlus.Services;
using ToDoListPlus.States;


namespace ToDoListPlus.ViewModels
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
            
        private readonly TaskManager _taskManager;
        private readonly AppStateService _appStateService;

        public IAsyncRelayCommand CleanUpCommand { get; }
        public IAsyncRelayCommand<ToDoItem> RemoveTaskCommand { get; }
        public IRelayCommand<ToDoItem> ToggleReadOnlyCommand { get; }
        public  ReadOnlyObservableCollection<ToDoItem> ToDoList => _taskManager.ToDoList;
        public int TotalTasks => _taskManager.TotalTasks;
        public int CompletedTasks => _taskManager.CompletedTasks;

        public ToDoListViewModel(TaskManager taskManager, AppStateService appStateService, IDialogService dialogService)
        {
            _taskManager = taskManager;
            _appStateService = appStateService;

            _taskManager.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(TaskManager.TotalTasks):
                        OnPropertyChanged(nameof(TotalTasks));
                        break;
                    case nameof(TaskManager.CompletedTasks):
                        OnPropertyChanged(nameof(CompletedTasks));
                        break;
                }
            };
            // Remove from here ??
            _appStateService.UserLoggedIn += OnUserLoggedIn;
            _appStateService.UserLoggedOut += OnUserLogOut;

            CleanUpCommand = new AsyncRelayCommand(CleanCompletedItems);
            RemoveTaskCommand = new AsyncRelayCommand<ToDoItem>(RemoveTask);
            ToggleReadOnlyCommand = new RelayCommand<ToDoItem>(ToggleReadOnly);
        }

        public void OnUserLoggedIn()
        {
            _taskManager.LoadToDoItems();
        }
        public void OnUserLogOut()
        {
            _taskManager.ClearTasks();
        }
        private void ToggleReadOnly(ToDoItem item)
        {
            item.IsReadOnly = !item.IsReadOnly;
        }
        private async Task RemoveTask(ToDoItem item)
        {
            await _taskManager.RemoveTask(item);
        }
        private async Task CleanCompletedItems()
        {
            await _taskManager.RemoveCompleteTask();
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}