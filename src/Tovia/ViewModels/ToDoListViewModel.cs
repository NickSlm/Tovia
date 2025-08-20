using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Tovia.Models;
using Tovia.Services;
using Tovia.States;


namespace Tovia.ViewModels
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {

        //Fields
        private readonly ITaskManager _taskManager;
        private readonly SettingsService _settingsService;
        private string _inProgressTaskColor;
        private string _failedTaskColor;
        private string _completedTaskColor;

        //Events
        public event PropertyChangedEventHandler? PropertyChanged;

        //Constructors
        public ToDoListViewModel(ITaskManager taskManager, IDialogService dialogService, SettingsService settingsService)
        {
            _taskManager = taskManager;
            _settingsService = settingsService;

            ApplySettings();
            _settingsService.SettingsChanged += (s,e) => ApplySettings();
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
            CleanUpCommand = new AsyncRelayCommand(CleanCompletedItems);
            RemoveTaskCommand = new AsyncRelayCommand<ToDoItem>(RemoveTask);
        }

        //Properties
        public int TotalTasks => _taskManager.TotalTasks;
        public int CompletedTasks => _taskManager.CompletedTasks;
        public  ReadOnlyObservableCollection<ToDoItem> ToDoList => _taskManager.ToDoList;
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

        //Commands
        public IAsyncRelayCommand CleanUpCommand { get; }
        public IAsyncRelayCommand<ToDoItem> RemoveTaskCommand { get; }

        //Methods
        private void ApplySettings()
        {
            var settings = _settingsService.userSettings;

            InProgressTaskColor = settings.Appearance.InProgressTask;
            FailedTaskColor = settings.Appearance.FailedTask;
            CompletedTaskColor = settings.Appearance.CompleteTask;
        }
        private async Task RemoveTask(ToDoItem item)
        {
            await _taskManager.RemoveTask(item);
        }
        private async Task CleanCompletedItems()
        {
            await _taskManager.RemoveCompleteTask();
        }
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}