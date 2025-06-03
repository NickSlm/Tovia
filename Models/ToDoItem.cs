using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using System.Windows.Input;
using ToDoListPlus.Models;

public class ToDoItem: INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

	private bool _isReadOnly = true;
	private bool _isComplete = false;
	private string _title;
	private string? _description;
	private DateTime? _dueDate;
	private string _eventId;
	private string _taskId;
	private string _importance;
	private double? _timeLeft = 0;
	private TaskState _status;


	public TaskState Status
	{
		get => _status;
		set
		{
			_status = value;
			OnPropertyChanged(nameof(Status));
			OnPropertyChanged(nameof(IsComplete));
		}
	}
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
		get => Status == TaskState.Complete;
		set
		{
			if (value)
			{
				Status = TaskState.Complete;
			}
			else if (Status == TaskState.Complete)
				Status = TaskState.InProgress;
			//When task status change => update the task in Microsoft using updateTask
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
	public double? TimeLeft
	{
		get => _timeLeft;
		set
		{
            _timeLeft = value;
			OnPropertyChanged(nameof(TimeLeft));
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
    public ToDoItem() {}

    public ToDoItem(string title, string? description, DateTime? dueDate ,string importance, string eventId, string taskId)
	{
		Title = title;
		Description = description;
		DueDate = dueDate;
		Importance = importance;
		EventId = eventId;
		TaskId = taskId;
    }

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	public EventHandler? OnCompletionChanged;
}
