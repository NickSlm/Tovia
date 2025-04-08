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

public class ToDoListViewModel: INotifyPropertyChanged
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
    private readonly ToDoContext _dbContext;
    private readonly AuthService _authService;

    private int _totalTasks;
    private int _completedTasks;
    private string _taskTitle = string.Empty;
    private string? _taskDescription = string.Empty;
    private DateTime? _taskDueDate = DateTime.Now;
    private bool _eventIsChecked;


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

    public ToDoListViewModel(ToDoContext dbContext, AuthService authService)
	{
        _dbContext = dbContext;
        _authService = authService;

        ToDoList = new ObservableCollection<ToDoItem>();
        EventIsChecked = false;


        ToDoList.CollectionChanged += (s, e) => UpdateTotalTasks();
        ToDoList.CollectionChanged += (s, e) => HandleCollectionChanged(e);

        _removeItemCommand = new DelegateCommand(RemoveItem, CanRemoveItem);
        _cleanItemsCommand = new DelegateCommand(CleanCompletedItems, CanExecute);
        _saveTaskCommand = new DelegateCommand(SaveTask, CanExecute);
        _toggleReadOnlyCommand = new DelegateCommand(ToggleReadOnly, CanExecute);

        LoadToDoItems();
        UpdateCompletedTasks();
    }

    private void LoadToDoItems()
    {
        var tasksFromDb = _dbContext.ToDoItems.ToList();

        ToDoList.Clear();

        foreach (var item in tasksFromDb)
        {
            item.PropertyChanged += Item_PropertyChanged;
            ToDoList.Add(item);
        }

        UpdateCompletedTasks();
        UpdateTotalTasks();

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

    private async void RemoveItem(object commandParameter)
    {
        if (commandParameter is ToDoItem item)
        {
            var itemToRemove = _dbContext.ToDoItems.FirstOrDefault(i => i.Id == item.Id);
            if (itemToRemove != null)
            {
                _dbContext.ToDoItems.Remove(itemToRemove);
            }
            if (item.EventId != null)
            {
                string result = await _authService.DeleteTaskAsync(item.EventId);
            }
            ToDoList.Remove(item);
        }
    }

    private bool CanRemoveItem(object commandParameter)
    {
        return commandParameter is ToDoItem item && ToDoList.Contains(item);
    }

    private async void CleanCompletedItems(object commandParameter)
    {
        if (commandParameter is ObservableCollection<ToDoItem> ToDoList)
        {
            for (int i = ToDoList.Count - 1; i >= 0; i--)
            {
                if (ToDoList[i].IsComplete)
                {
                    if (ToDoList[i].EventId != null)
                    {
                        string result = await _authService.DeleteTaskAsync(ToDoList[i].EventId);
                    }
                    _dbContext.ToDoItems.Remove(ToDoList[i]);
                    ToDoList.RemoveAt(i);
                }
            }
        }
    }

    private void ToggleReadOnly(object commandParameter)
    {
        if (commandParameter is ToDoItem item)
        {
            item.IsReadOnly = !item.IsReadOnly;

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

        string eventId = string.Empty;

        if (EventIsChecked)
        {
            eventId = await _authService.PostTaskAsync(TaskTitle, TaskDescription, TaskDueDate);
        }

        var newTask = new ToDoItem
        {
            Title = TaskTitle,
            Description = TaskDescription,
            DueDate = TaskDueDate,
            IsComplete = false,
            IsReadOnly = true,
            EventId = eventId
        };

        _dbContext.ToDoItems.Add(newTask);
        ToDoList.Add(newTask);

        //Reset Form Fields
        TaskTitle = string.Empty;
        TaskDescription = string.Empty;
        TaskDueDate = DateTime.Now;
        EventIsChecked = false;
    }

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}
