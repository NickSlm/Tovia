using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ToDoItem : INotifyPropertyChanged
{

    private string _title;
    private bool _isReadOnly;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        set
        {
            _isReadOnly = value;
            OnPropertyChanged();
        }
    }

	public ToDoItem(string title)
	{
        Title = title;
        IsReadOnly = true
	}


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
