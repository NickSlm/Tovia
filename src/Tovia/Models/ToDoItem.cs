using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tovia.Models;

public class ToDoItem: INotifyPropertyChanged
{
	public string TaskId;
	private string _title;
	private string _importance;
	private string? _description;
	private string? _eventId;
	private DateTime? _dueDate;
	private TimeSpan? _timeLeft;
	private bool _isComplete;

    public event PropertyChangedEventHandler? PropertyChanged;
	public EventHandler? OnCompletionChanged;
    public ToDoItem() {}

    public ToDoItem(string title, string? description, DateTime? dueDate ,string importance)
	{
		Title = title;
		Description = description;
		DueDate = dueDate;
		Importance = importance;
    }

	public int Id { get; set; }
	public TaskState Status
	{
		get
		{
			if (_isComplete) return TaskState.Complete;
			if (DueDate.HasValue && DueDate < DateTime.Now) return TaskState.Failed;
			return TaskState.InProgress;
		}
	}
	public bool IsComplete
	{
		get => _isComplete;
		set
		{
			if (_isComplete != value)
			{
				_isComplete = value;
			}
			OnPropertyChanged(nameof(IsComplete));
			OnPropertyChanged(nameof(Status));
			OnCompletionChanged?.Invoke(this, EventArgs.Empty);
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
	public string Importance
	{
		get => _importance;
		set
		{
			_importance = value;
			OnPropertyChanged(nameof(Importance));
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
	public TimeSpan? TimeLeft
	{
		get => _timeLeft;
		set
		{
            _timeLeft = value;
			OnPropertyChanged(nameof(TimeLeft));
		}
	}
	public DateTime LastTimeModified { get; set; }
    public int prioritySortOrder
    {
        get
        {
            return Importance switch
            {
                "high" => 1,
                "normal" => 2,
                "low" => 3
            };
        }
    }
	public bool SoftDelete
	{
		get;
		set;
	} = false;
    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
