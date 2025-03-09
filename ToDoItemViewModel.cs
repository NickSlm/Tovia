using System;
using System.ComponentModel;
using System.Windows.Input;

public class ToDoItemViewModel: INotifyPropertyChanged
{



	public event PropertyChangedEventHandler? PropertyChanged;

	public ICommand ToggleReadOnlyCommand => _toggleReadOnlyCommand;

	private DelegateCommand _toggleReadOnlyCommand;

	private bool _isReadOnly;
	public bool IsReadOnly
	{
		get => _isReadOnly;
		set
		{
			_isReadOnly = value;
			OnPropertyChanged(nameof(IsReadOnly));
		}
	}

	private string? _title;
	public string Title {
		get => _title;
		set
		{
			_title = value;
			OnPropertyChanged(nameof(Title));
		}
	}

	public ToDoItemViewModel()
	{
		_title = "New Task";
		_isReadOnly = true;

		_toggleReadOnlyCommand = new DelegateCommand(ToggleReadOnly, canToggle);

	}

	public ToDoItemViewModel(string title)
	{
		Title = title;

	}

	private void ToggleReadOnly(object commandParameter)
	{
		IsReadOnly = !IsReadOnly;
	}

	private bool canToggle(object commmandParameter)
	{
		return true;
	}

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


}
