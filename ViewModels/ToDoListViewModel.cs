using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ToDoListPlus.Services;
using ToDoListPlus.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;



namespace ToDoListPlus.ViewModels
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand RemoveItemCommand => _removeItemCommand;
        public ICommand CleanItemsCommand => _cleanItemsCommand;
        public ICommand SaveTaskCommand => _saveTaskCommand;
        public ICommand ToggleReadOnlyCommand => _toggleReadOnlyCommand;

        private readonly DelegateCommand _toggleReadOnlyCommand;
        private readonly DelegateCommand _removeItemCommand;
        private readonly DelegateCommand _cleanItemsCommand;
        private readonly DelegateCommand _saveTaskCommand;
        private readonly AuthService _authService;
        private readonly TaskService _taskService;
        private readonly AppStateService _appStateService;

        private int _totalTasks;
        private int _completedTasks;
        private string _taskTitle = string.Empty;
        private string? _taskDescription = string.Empty;
        private DateTime? _taskDueDate = DateTime.Now;
        private bool _eventIsChecked = false;
        private string _taskImportance = string.Empty;


        public ObservableCollection<ToDoItem> ToDoList { get; set; }
        public int TotalTasks
        {
            get => _totalTasks;
            set
            {
                _totalTasks = value;
                OnPropertyChanged(nameof(TotalTasks));
            }
        }
        public int CompletedTasks
        {
            get => _completedTasks;
            set
            {
                _completedTasks = value;
                OnPropertyChanged(nameof(CompletedTasks));
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


        public ToDoListViewModel(AuthService authService, TaskService taskService, AppStateService appStateService)
        {
            _authService = authService;
            _taskService = taskService;
            _appStateService = appStateService;

            ToDoList = new ObservableCollection<ToDoItem>();

            ToDoList.CollectionChanged += (s, e) => UpdateTotalTasks();
            ToDoList.CollectionChanged += (s, e) => HandleCollectionChanged(e);

            _appStateService.UserLoggedIn += OnUserLoggedIn;

            _removeItemCommand = new DelegateCommand(RemoveItem, CanRemoveItem);
            _cleanItemsCommand = new DelegateCommand(CleanCompletedItems, CanExecute);
            _saveTaskCommand = new DelegateCommand(SaveTask, CanExecute);
            _toggleReadOnlyCommand = new DelegateCommand(ToggleReadOnly, CanExecute);

            UpdateCompletedTasks();
        }

        public void OnUserLoggedIn()
        {
            LoadToDoItems();
        }
        public async void LoadToDoItems()
        {
            var tasks = await _taskService.GetTasksAsync();

            ToDoList.Clear();
            foreach (var task in tasks)
            {
                task.OnCompletionChanged += async (s, e) => await UpdateTaskAsync((ToDoItem)s);
                ToDoList.Add(task);
            }

            UpdateCompletedTasks();
            UpdateTotalTasks();
        }
        private async Task UpdateTaskAsync(ToDoItem task)
        {
            try
            {
                await _taskService.UpdateTaskAsync(task.TaskId, task.IsComplete);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void UpdateTotalTasks()
        {
            TotalTasks = ToDoList.Count;
        }
        private void UpdateCompletedTasks()
        {
            CompletedTasks = (TotalTasks > 0) ? (ToDoList.Count(item => item.IsComplete) * 100) / TotalTasks : 0;

        }
        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ToDoItem.IsComplete))
            {
                UpdateCompletedTasks();
            }
        }
        private void HandleCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (ToDoItem newItem in e.NewItems)
                {
                    newItem.PropertyChanged += Item_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (ToDoItem newItem in e.OldItems)
                {
                    newItem.PropertyChanged -= Item_PropertyChanged;
                }
            }
            UpdateCompletedTasks();
        }
        private bool CanExecute(object commandParameter)
        {
            //return !string.IsNullOrWhiteSpace(ItemText);
            return true;
        }
        private bool CanRemoveItem(object commandParameter)
        {
            return commandParameter is ToDoItem item && ToDoList.Contains(item);
        }
        private void ToggleReadOnly(object commandParameter)
        {
            if (commandParameter is ToDoItem item)
            {
                item.IsReadOnly = !item.IsReadOnly;
            }
        }
        private async void RemoveItem(object commandParameter)
        {
            if (commandParameter is ToDoItem item)
            {
                if (!string.IsNullOrEmpty(item.EventId))
                {
                    string eventResult = await _taskService.DeleteEventAsync(item.EventId);
                    MessageBox.Show(eventResult);
                }
                ToDoList.Remove(item);
                string taskResult = await _taskService.DeleteTaskAsync(item.TaskId);
                MessageBox.Show(taskResult);

            }
        }
        private async void CleanCompletedItems(object commandParameter)
        {
            if (commandParameter is ObservableCollection<ToDoItem> ToDoList)
            {
                for (int i = ToDoList.Count - 1; i >= 0; i--)
                {
                    if (ToDoList[i].IsComplete)
                    {
                        if (!string.IsNullOrEmpty(ToDoList[i].EventId))
                        {
                            await _taskService.DeleteEventAsync(ToDoList[i].EventId);
                        }
                        await _taskService.DeleteTaskAsync(ToDoList[i].TaskId);
                        ToDoList.RemoveAt(i);
                    }
                }
            }
        }
        private async void SaveTask(object commandParameter)
        {
            if (string.IsNullOrWhiteSpace(TaskTitle))
            {
                MessageBox.Show("Title Required");
                return;
            }
            if (!TaskDueDate.HasValue)
            {
                MessageBox.Show("DueTime Required");
                return;
            }
            if (string.IsNullOrEmpty(TaskImportance))
            {
                MessageBox.Show("Priority Required");
                return;
            }

            ToDoItem newTask = await _taskService.CreateTaskAsync(TaskTitle, TaskDescription, TaskDueDate, TaskImportance, EventIsChecked);
            ToDoList.Add(newTask);

            //Reset Form Fields
            TaskTitle = string.Empty;
            TaskDescription = string.Empty;
            TaskDueDate = DateTime.Now;
            TaskImportance = string.Empty;
            EventIsChecked = false;
        }
        public void Reset()
        {
            ToDoList.Clear();
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}