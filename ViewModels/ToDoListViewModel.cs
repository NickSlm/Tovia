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
        private readonly SettingsService _settingsService;
        private string _inProgressTaskColor { get; set; }
        private string _failedTaskColor { get; set; }
        private string _completedTaskColor { get; set; }

        public IAsyncRelayCommand CleanUpCommand { get; }
        public IAsyncRelayCommand<ToDoItem> RemoveTaskCommand { get; }
        public IRelayCommand<ToDoItem> ToggleReadOnlyCommand { get; }
        public  ReadOnlyObservableCollection<ToDoItem> ToDoList => _taskManager.ToDoList;
        public int TotalTasks => _taskManager.TotalTasks;
        public int CompletedTasks => _taskManager.CompletedTasks;
        public string InProgressTaskColor
        {
            get => _inProgressTaskColor;
            set
            {
                _inProgressTaskColor = value;
                OnPropertyChanged(nameof(InProgressTaskColor));
            }
        }
        public string FailedTaskColor
        {
            get => _failedTaskColor;
            set
            {
                _failedTaskColor = value;
                OnPropertyChanged(nameof(FailedTaskColor));
            }
        }
        public string CompletedTaskColor
        {
            get => _completedTaskColor;
            set
            {
                _completedTaskColor = value;
                OnPropertyChanged(nameof(CompletedTaskColor));
            }
        }
        public ToDoListViewModel(TaskManager taskManager, IDialogService dialogService, SettingsService settingsService)
        {
            _taskManager = taskManager;
            _settingsService = settingsService;

            settingsService.Load();
            var settings = settingsService.userSettings;

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

            InProgressTaskColor = settings.Appearance.InProgressTask;
            FailedTaskColor = settings.Appearance.FailedTask;
            CompletedTaskColor = settings.Appearance.CompleteTask;

            CleanUpCommand = new AsyncRelayCommand(CleanCompletedItems);
            RemoveTaskCommand = new AsyncRelayCommand<ToDoItem>(RemoveTask);
            ToggleReadOnlyCommand = new RelayCommand<ToDoItem>(ToggleReadOnly);
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