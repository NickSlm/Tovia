using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using System.Windows.Input;

public class ToDoItem: INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;
	public ICommand ToggleReadOnlyCommand => _toggleReadOnlyCommand;
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	private DelegateCommand _toggleReadOnlyCommand;

	private bool _isReadOnly;
	private bool _isComplete;
	private string? _title;
	private string? _description;


	public bool IsReadOnly
	{
		get => _isReadOnly;
		set
		{
			_isReadOnly = value;
			OnPropertyChanged(nameof(IsReadOnly));
		}
	}
	public bool IsComplete
	{
		get => _isComplete;
		set
		{
			_isComplete = value;
			OnPropertyChanged(nameof(IsComplete));
		}
	}
	public string Title {
		get => _title;
		set
		{
			_title = value;
			OnPropertyChanged(nameof(Title));
		}
	}

	public string Description
	{
		get => _description;
		set
		{
			_description = value;
			OnPropertyChanged(nameof(Description));
		}
	}

    public ToDoItem() { }

    public ToDoItem(string title, string description)
	{
		Title = title;
		Description = description;
        _isReadOnly = true;
        _isComplete = false;
        _toggleReadOnlyCommand = new DelegateCommand(ToggleReadOnly, canToggle);
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
