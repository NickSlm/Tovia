using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

public class ToDoListViewModel: INotifyPropertyChanged
{
    public ICommand AddItemCommand => _addItemCommand;
    public ICommand RemoveItemCommand => _removeItemCommand;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<ToDoItemViewModel> ToDoList { get; set; }

    private readonly DelegateCommand _addItemCommand;
    private readonly DelegateCommand _removeItemCommand;

    private string _itemText;
    public string ItemText
    {
        get => _itemText;
        set { _itemText = value; OnPropertyChanged(nameof(ItemText)); }
    }

    public ToDoListViewModel()
	{
        ToDoList = new ObservableCollection<ToDoItemViewModel>
        {
            new ToDoItemViewModel(),
        };
        _addItemCommand = new DelegateCommand(AddItem, CanAddItem);
        _removeItemCommand = new DelegateCommand(RemoveItem, CanRemoveItem);
    }


    private void AddItem(object commandParameter)
    {
        ToDoList.Add(new ToDoItemViewModel());
        _addItemCommand.InvokeCanExecuteChanged();
    }

    private bool CanAddItem(object commandParameter)
    {
        return true;
    }

    private void RemoveItem(object commandParameter)
    {
        ToDoList.Remove((ToDoItemViewModel)commandParameter);
        _addItemCommand.InvokeCanExecuteChanged();
    }

    private bool CanRemoveItem(object commandParameter)
    {
        return commandParameter is ToDoItemViewModel item && ToDoList.Contains(item);
    }

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


}
