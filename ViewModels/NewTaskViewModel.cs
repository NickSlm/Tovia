using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows;
using ToDoListPlus.Services;

namespace ToDoListPlus.ViewModels
{
    public class NewTaskViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly TaskService _taskService;
        private readonly IDialogService _dialogService;
        private string _taskTitle = string.Empty;
        private string _taskDescription = string.Empty;
        private DateTime? _taskDueDate = DateTime.Now;
        private bool _eventIsChecked = false;
        private string _taskImportance = string.Empty;

        public IAsyncRelayCommand SaveTaskCommand { get; }
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

        public NewTaskViewModel(TaskService taskService, IDialogService dialogService)
        {
            _taskService = taskService;
            _dialogService = dialogService;
            SaveTaskCommand = new AsyncRelayCommand(SaveTask);
        }

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
            ToDoItem newTask = await _taskService.CreateTaskAsync(TaskTitle, TaskDescription, TaskDueDate, TaskImportance, EventIsChecked);
            newTask.OnCompletionChanged += async (s, e) => {
                var t = (ToDoItem)s;
                try
                {
                    await _taskService.UpdateTaskAsync(t.TaskId, t.IsComplete);
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage($"{ex.Message}", "Error");
                }
            };
            _taskService.ToDoList.Add(newTask);

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
