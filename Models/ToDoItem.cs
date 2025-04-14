using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using System.Windows.Input;

public class ToDoItem: INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	private bool _isReadOnly = true;
	private bool _isComplete = false;
	private string _title;
	private string? _description;
	private DateTime? _dueDate;
	private string? _eventId;
	private string _priority;


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
	public string? Description
	{
		get => _description;
		set
		{
			_description = value;
			OnPropertyChanged(nameof(Description));
		}
	}
	public DateTime? DueDate
	{
		get => _dueDate;
		set
		{
			_dueDate = value;
			OnPropertyChanged(nameof(DueDate));
		}
	}
	public string? EventId
	{
		get => _eventId;
		set
		{
			_eventId = value;
			OnPropertyChanged(nameof(EventId));
		}
	}
	public string Priority
	{
		get => _priority;
		set
		{
			_priority = value;
			OnPropertyChanged(nameof(Priority));
		}
	}

	public ToDoItem() { }

    public ToDoItem(string title, string? description, DateTime? dueDate, string? eventId, string priority)
	{
		Title = title;
		Description = description;
		DueDate = dueDate;
		EventId = eventId;
		Priority = priority;
    }

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
