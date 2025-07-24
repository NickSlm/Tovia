using System.ComponentModel;
using ToDoListPlus.Models;

public class ToDoItem: INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

	private bool _isReadOnly = true;
	private string _title;
	private string? _description;
	private DateTime? _dueDate;
	private string _importance;
	private TimeSpan? _timeLeft;
	private bool _isComplete;
	private string _eventId;
	private string _taskId;
	public bool IsReadOnly
	{
		get => _isReadOnly;
		set
		{
			_isReadOnly = value;
			OnPropertyChanged(nameof(IsReadOnly));
		}
	}
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
	public TimeSpan? TimeLeft
	{
		get => _timeLeft;
		set
		{
            _timeLeft = value;
			OnPropertyChanged(nameof(TimeLeft));
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
    public string EventId
    {
        get => _eventId;
        set
        {
            _eventId = value;
            OnPropertyChanged(nameof(EventId));
        }
    }
    public string TaskId
    {
        get => _taskId;
        set
        {
            _taskId = value;
            OnPropertyChanged(nameof(TaskId));
        }
    }
    public ToDoItem() {}

    public ToDoItem(string title, string? description, DateTime? dueDate ,string importance)
	{
		Title = title;
		Description = description;
		DueDate = dueDate;
		Importance = importance;
    }

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	public EventHandler? OnCompletionChanged;
}
