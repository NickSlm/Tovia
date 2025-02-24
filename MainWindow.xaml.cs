using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToDoListPlus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
/// 


public partial class MainWindow : INotifyPropertyChanged
{
    private ObservableCollection<ToDoItem> _toDoList = new ObservableCollection<ToDoItem>();

    public ObservableCollection<ToDoItem> ToDoList
    {
        get => _toDoList;
        set
        {
            _toDoList = value;
            OnPropertyChanged();
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        ToDoList = new ObservableCollection<ToDoItem>
        {
            new ToDoItem("Title 1"),
            new ToDoItem("Title 2"),
            new ToDoItem("Title 3")
        };

    }

    private void RemoveFromList(ToDoItem item)
    {
        if (item != null)
        {
            ToDoList.Remove(item);
        }
    }

    private void ToggleReadOnly(ToDoItem item)
    {
        if (item != null)
        {
            item.IsReadOnly = !item.IsReadOnly;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}