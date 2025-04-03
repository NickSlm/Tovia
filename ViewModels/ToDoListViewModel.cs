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

public class ToDoListViewModel: INotifyPropertyChanged
{


    public event PropertyChangedEventHandler? PropertyChanged;
    public ICommand AddItemCommand => _addItemCommand;
    public ICommand RemoveItemCommand => _removeItemCommand;
    public ICommand CleanItemsCommand => _cleanItemsCommand;
    public ICommand SaveTaskCommand => _saveTaskCommand;
    public ICommand ToggleReadOnlyCommand => _toggleReadOnlyCommand;


    private readonly DelegateCommand _toggleReadOnlyCommand;
    private readonly DelegateCommand _addItemCommand;
    private readonly DelegateCommand _removeItemCommand;
    private readonly DelegateCommand _cleanItemsCommand;
    private readonly DelegateCommand _saveTaskCommand;
    private readonly ToDoContext _dbContext;

    private int _totalTasks;
    private int _completedTasks;
    private string _taskTitle;
    private string _taskDescription;
    private DateTime _taskDueDate;


    public ObservableCollection<ToDoItem> ToDoList { get; set; }

    public string TaskTitle
    {
        get => _taskTitle;
        set
        {
            _taskTitle = value;
            OnPropertyChanged(nameof(TaskTitle));
        }
    }
    public string TaskDescription
    {
        get => _taskDescription;
        set
        {
            _taskDescription = value;
            OnPropertyChanged(nameof(TaskDescription));
        }
    }

    public DateTime TaskDueDate
    {
        get => _taskDueDate;
        set
        {
            _taskDueDate = value;
            OnPropertyChanged(nameof(TaskDueDate));
        }
    }
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


    public ToDoListViewModel(ToDoContext dbContext)
	{
        _dbContext = dbContext;

        ToDoList = new ObservableCollection<ToDoItem>();

        LoadToDoItems();


        ToDoList.CollectionChanged += (s, e) => UpdateTotalTasks();
        ToDoList.CollectionChanged += (s, e) => HandleCollectionChanged(e);

        _removeItemCommand = new DelegateCommand(RemoveItem, CanRemoveItem);
        _cleanItemsCommand = new DelegateCommand(CleanCompletedItems, CanExecute);
        _saveTaskCommand = new DelegateCommand(SaveTask, CanExecute);
        _toggleReadOnlyCommand = new DelegateCommand(ToggleReadOnly, CanExecute);

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

        TotalTasks = ToDoList.Count();
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

    private void RemoveItem(object commandParameter)
    {
        if (commandParameter is ToDoItem item)
        {
            var itemToRemove = _dbContext.ToDoItems.FirstOrDefault(i => i.Id == item.Id);
            if (itemToRemove != null)
            {
                _dbContext.ToDoItems.Remove(itemToRemove);
            }
            ToDoList.Remove(item);
        }
    }

    private bool CanRemoveItem(object commandParameter)
    {
        return commandParameter is ToDoItem item && ToDoList.Contains(item);
    }

    private void CleanCompletedItems(object commandParameter)
    {
        if (commandParameter is ObservableCollection<ToDoItem> ToDoList)
        {
            for (int i = ToDoList.Count - 1; i >= 0; i--)
            {
                if (ToDoList[i].IsComplete)
                {
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

    private void SaveTask(object commandParameter)
    {

        var newTask = new ToDoItem(TaskTitle, TaskDescription, _taskDueDate);
        _dbContext.ToDoItems.Add(newTask);
        ToDoList.Add(newTask);
    }

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}
