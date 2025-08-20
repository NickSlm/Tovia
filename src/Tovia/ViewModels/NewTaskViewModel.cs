using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Tovia.Services;
using Tovia.States;

namespace Tovia.ViewModels
{
    public class NewTaskViewModel: INotifyPropertyChanged
    {
        //Fields
        private readonly IMicrosoftGraphService _taskService;
        private readonly ITaskManager _taskManager;
        private readonly IDialogService _dialogService;
        private string _taskTitle = string.Empty;
        private string _taskDescription = string.Empty;
        private DateTime? _taskDueDate = DateTime.Now;
        private bool _eventIsChecked = false;
        private string _taskImportance = string.Empty;

        //Events
        public event PropertyChangedEventHandler? PropertyChanged;

        //Constructor
        public NewTaskViewModel(IMicrosoftGraphService taskService, ITaskManager taskManager ,IDialogService dialogService)
        {
            _taskService = taskService;
            _taskManager = taskManager;
            _dialogService = dialogService;
            SaveTaskCommand = new AsyncRelayCommand(SaveTask);
        }

        //Properties
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
        public bool EventIsChecked
        {
            get => _eventIsChecked;
            set
            {
                _eventIsChecked = value;
                OnPropertyChanged(nameof(EventIsChecked));
            }
        }
        public string TaskImportance
        {
            get => _taskImportance;
            set
            {
                if (_taskImportance != null)
                {
                    _taskImportance = value;
                    OnPropertyChanged(nameof(TaskImportance));
                }
            }
        }

        //Commands
        public IAsyncRelayCommand SaveTaskCommand { get; }

        private async Task SaveTask()
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
            if (string.IsNullOrEmpty(TaskImportance))
            {
                _dialogService.ShowMessage("Priority Required", "Warning");
                return;
            }

            var newTask = new ToDoItem
            {
                Title = TaskTitle,
                Description = TaskDescription,
                DueDate = TaskDueDate,
                Importance = TaskImportance
            };

            await _taskManager.SaveTask(newTask, EventIsChecked);

            //Reset Form Fields
            TaskTitle = string.Empty;
            TaskDescription = string.Empty;
            TaskDueDate = DateTime.Now;
            TaskImportance = string.Empty;
            EventIsChecked = false;
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
