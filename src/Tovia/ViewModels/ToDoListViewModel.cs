using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Tovia.interfaces;
using Tovia.Models;
using Tovia.Services;
using Tovia.States;


namespace Tovia.ViewModels
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        private readonly ITaskManager _taskManager;
        private readonly SettingsService _settingsService;
        private readonly IConfiguration _config;
        private readonly IDialogService _dialogService;

        private string _taskTitle = string.Empty;
        private string? _taskDescription = string.Empty;
        private DateTime? _taskDueDate = DateTime.Now;
        private string _taskPriority = string.Empty;
        private bool _taskEvent = false;
        private bool _isSyncing;

        private string _inProgressTaskColor;
        private string _failedTaskColor;
        private string _completedTaskColor;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ToDoListViewModel(ITaskManager taskManager, IDialogService dialogService, SettingsService settingsService, IConfiguration config)
        {
            _taskManager = taskManager;
            _settingsService = settingsService;
            _config = config;
            _dialogService = dialogService;

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
            CreateTaskCommand = new AsyncRelayCommand(CreateTask);
        }

        public int TotalTasks => _taskManager.TotalTasks;
        public int CompletedTasks => _taskManager.CompletedTasks;

        public  ReadOnlyObservableCollection<ToDoItem> ToDoList => _taskManager.ToDoList;
        public ObservableCollection<PriorityItem> Priorities { get; } = new()
        {
            new PriorityItem { Name = "high", Color = Brushes.Red },
            new PriorityItem { Name = "normal", Color = Brushes.Orange },
            new PriorityItem { Name = "low", Color = Brushes.Green },
        };

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
        public string TaskTitle
        {
            get => _taskTitle;
            set
            {
                _taskTitle = value;
                OnPropertyChanged(nameof(TaskTitle));
            }
        }
        public string? TaskDescription
        {
            get => _taskDescription;
            set
            {
                _taskDescription = value;
                OnPropertyChanged(nameof(TaskDescription));
            }
        }
        public DateTime? TaskDueDate
        {
            get => _taskDueDate;
            set
            {
                _taskDueDate = value;
                OnPropertyChanged(nameof(TaskDueDate));
            }
        }
        public string TaskPriority
        {
            get => _taskPriority;
            set
            {
                _taskPriority = value;
                OnPropertyChanged(nameof(TaskPriority));
            }
        }
        public bool TaskEvent
        {
            get => _taskEvent;
            set
            {
                _taskEvent = value;
                OnPropertyChanged(nameof(TaskEvent));
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

        public IAsyncRelayCommand CleanUpCommand { get; }
        public IAsyncRelayCommand<ToDoItem> RemoveTaskCommand { get; }
        public IAsyncRelayCommand CreateTaskCommand { get; }

        private void ApplySettings()
        {
            var settings = _config.GetSection("Appearance").Get<AppearanceSettings>();

            InProgressTaskColor = settings.InProgressTask;
            FailedTaskColor = settings.FailedTask;
            CompletedTaskColor = settings.CompleteTask;
        }
        private async Task CreateTask()
        {
            if (string.IsNullOrWhiteSpace(TaskTitle))
            {
                _dialogService.ShowMessage("Title Required", "Warning");
                return;
            }
            if (!TaskDueDate.HasValue)
            {
                _dialogService.ShowMessage("Due Date Required", "Warning");
                return;
            }
            if (string.IsNullOrEmpty(TaskPriority))
            {
                _dialogService.ShowMessage("Priority Required", "Warning");
                return;
            }

            var newTask = new ToDoItem
            {
                Title = TaskTitle,
                Description = TaskDescription,
                DueDate = TaskDueDate,
                Importance = TaskPriority,
            };

            await _taskManager.SaveTask(newTask, TaskEvent);

            TaskTitle = string.Empty;
            TaskDescription = string.Empty;
            TaskDueDate = DateTime.Now;
            TaskPriority = string.Empty;
            TaskEvent = false;
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