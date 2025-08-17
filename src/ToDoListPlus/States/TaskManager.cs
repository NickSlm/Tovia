using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using ToDoListPlus.Models;
using ToDoListPlus.Services;

namespace ToDoListPlus.States
{
    public class TaskManager: ITaskManager
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IMicrosoftGraphService _taskService;
        private readonly ObservableCollection<ToDoItem> _toDoList = new();
        private int _completedTasks;

        internal ObservableCollection<ToDoItem> ToDoListInternal => _toDoList;

        public ReadOnlyObservableCollection<ToDoItem> ToDoList { get; }

        public int TotalTasks
        {
            get => ToDoList.Count;
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
        public TaskManager(IMicrosoftGraphService taskService)
        {
            _taskService = taskService;
            ToDoList = new ReadOnlyObservableCollection<ToDoItem>(_toDoList);

            _toDoList.CollectionChanged += (s, e) => HandleCollectionChanged(e);
            _toDoList.CollectionChanged += (s, e) => UpdateCompletedTasks();
            _toDoList.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TotalTasks));
            };

            LoadToDoItems();
        }

        public async Task LoadToDoItems()
        {
            _toDoList.Clear();
            var tasks = await _taskService.GetTasksAsync();
            foreach (var task in tasks)
            {
                task.OnCompletionChanged += async (s, e) =>
                {
                    var t = (ToDoItem)task;
                    try
                    {
                        await _taskService.UpdateTaskAsync(t.TaskId, t.IsComplete);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error updating Task {t.TaskId}: {ex}");
                    }
                };
                _toDoList.Add(task);
            }
        }
        public async Task RemoveTask(ToDoItem item)
        {
            if (!string.IsNullOrEmpty(item.EventId))
            {
                await _taskService.DeleteEventAsync(item.EventId);
            }
            await _taskService.DeleteTaskAsync(item.TaskId);
            _toDoList.Remove(item);
        }
        public async Task RemoveCompleteTask()
        {
            for (int i = ToDoList.Count - 1; i >= 0; i--)
            {
                if (ToDoList[i].Status == TaskState.Complete || ToDoList[i].Status == TaskState.Failed)
                {
                    if (!string.IsNullOrEmpty(ToDoList[i].EventId))
                    {
                        await _taskService.DeleteEventAsync(ToDoList[i].EventId);
                    }
                    await _taskService.DeleteTaskAsync(ToDoList[i].TaskId);
                    _toDoList.RemoveAt(i);
                }
            }
        }
        public void ClearTasks()
        {
            _toDoList.Clear();
        }
        public async Task SaveTask(ToDoItem item, bool createEvent)
        {
            ToDoItem newTask = await _taskService.CreateTaskAsync(item, createEvent);

            newTask.OnCompletionChanged += async (s, e) =>
            {
                var t = (ToDoItem)s;
                try
                {
                    await _taskService.UpdateTaskAsync(t.TaskId, t.IsComplete);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating Task {t.TaskId}: {ex}");
                }
            };
            _toDoList.Add(newTask);
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
            //Subscribing the Item_PropertyChanged method to each Task
            //which updates the CompletedTasks based on the checkbox
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (ToDoItem newItem in e.NewItems)
                {
                    newItem.PropertyChanged += Item_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (ToDoItem newItem in e.OldItems)
                {
                    newItem.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }
        private void UpdateCompletedTasks()
        {
            CompletedTasks = (TotalTasks > 0) ? (ToDoList.Count(item => item.IsComplete) * 100) / TotalTasks : 0;
        }
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}

