using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    public ICommand OpenPopupCommand => _openPopupCommand;


    private readonly DelegateCommand _addItemCommand;
    private readonly DelegateCommand _removeItemCommand;
    private readonly DelegateCommand _cleanItemsCommand;
    private readonly DelegateCommand _openPopupCommand;
    private readonly IDialogService _dialogService;

    private string _itemText;
    private int _totalTasks;
    private int _completedTasks;

    public ObservableCollection<ToDoItem> ToDoList { get; set; }
    public string ItemText
    {
        get => _itemText;
        set
        { 
            _itemText = value;
            OnPropertyChanged(nameof(ItemText));
            _addItemCommand.InvokeCanExecuteChanged();
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


    public ToDoListViewModel(IDialogService dialogService)
	{
        ToDoList = new ObservableCollection<ToDoItem>();

        ToDoList.CollectionChanged += (s, e) => UpdateTotalTasks();
        ToDoList.CollectionChanged += (s, e) => HandleCollectionChanged(e);

        _dialogService = dialogService;
        _addItemCommand = new DelegateCommand(AddItem, CanExecute);
        _removeItemCommand = new DelegateCommand(RemoveItem, CanRemoveItem);
        _cleanItemsCommand = new DelegateCommand(CleanCompletedItems, CanExecute);
        _openPopupCommand = new DelegateCommand(OpenPopup, CanExecute);

        UpdateCompletedTasks();
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
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (ToDoItem newItem in e.NewItems)
            {
                newItem.PropertyChanged += Item_PropertyChanged;
            }
        }
        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (ToDoItem newItem in e.OldItems)
            {
                newItem.PropertyChanged -= Item_PropertyChanged;
            }
        }
        UpdateCompletedTasks();
    }

    private void AddItem(object commandParameter)
    {
        ToDoList.Add(new ToDoItem(ItemText));
        ItemText = "";
        _addItemCommand.InvokeCanExecuteChanged();
    }

    private bool CanExecute(object commandParameter)
    {
        //return !string.IsNullOrWhiteSpace(ItemText);
        return true;
    }

    private void RemoveItem(object commandParameter)
    {
        ToDoList.Remove((ToDoItem)commandParameter);
        _addItemCommand.InvokeCanExecuteChanged();
    }

    private bool CanRemoveItem(object commandParameter)
    {
        return commandParameter is ToDoItem item && ToDoList.Contains(item);
    }

    private void CleanCompletedItems(object commandParameter)
    {
        MessageBox.Show("Tasks Cleaned");
        if (commandParameter is ObservableCollection<ToDoItem> ToDoList)
        {
            for (int i = ToDoList.Count() - 1; i >= 0; i--)
            {
                if (ToDoList[i].IsComplete)
                {
                    ToDoList.RemoveAt(i);
                }
            }
        }
    }

    private void OpenPopup(object commandParameter)
    {
        var dialogViewModel = new PopupDialogViewModel();
        var result = _dialogService.ShowDialog(dialogViewModel);
        if (result == true)
        {
            ToDoList.Add(new ToDoItem(dialogViewModel.TaskTitle));
        }
    }

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}
