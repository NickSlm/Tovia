using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Tovia.Data;
using Tovia.interfaces;
using Tovia.Models;
using Tovia.Services;

namespace Tovia.States
{
    public class TaskManager: ITaskManager
    {
        private readonly ILocalDBService _localDBService;
        private readonly AppStateService _appStateService;
        private readonly ObservableCollection<ToDoItem> _toDoList = new();
        private int _completedTasks;

        public event PropertyChangedEventHandler? PropertyChanged;

        public TaskManager(ILocalDBService localDBService, AppStateService appStateService)
        {
            _localDBService = localDBService;
            _appStateService = appStateService;
            ToDoList = new ReadOnlyObservableCollection<ToDoItem>(_toDoList);

            _toDoList.CollectionChanged += (s, e) => HandleCollectionChanged(e);
            _toDoList.CollectionChanged += (s, e) => UpdateCompletedTasks();
            _toDoList.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TotalTasks));
            };
        }

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

        public async Task LoadToDoItems()
        {
            _toDoList.Clear();
            var tasks = await _appStateService.TaskProvider.GetTasksAsync();
            foreach (var task in tasks)
            {
                task.OnCompletionChanged += async (s, e) =>
                {
                    var t = task;
                    try
                    {
                        await _appStateService.TaskProvider.UpdateTaskAsync(t.TaskId, t.IsComplete);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error updating Task {t.TaskId}: {ex}");
                    }
                };
                _toDoList.Add(task);
            }
        }
        public async Task SaveTask(ToDoItem item, bool createEvent)
        {

            ToDoItem newTask = await _appStateService.TaskProvider.CreateTaskAsync(item, createEvent);

            UsersTasks task = new UsersTasks()
            {
                Title = newTask.Title,
                Description = newTask.Description,
                DueDate = newTask.DueDate,
                TaskId = newTask.TaskId,
                EventId = newTask.EventId,
                Priority = newTask.Importance,
                Status = newTask.Status,
                SoftDelete = newTask.SoftDelete
            };

            await _localDBService.SaveTask(task);

            newTask.OnCompletionChanged += async (s, e) =>
            {
                var t = (ToDoItem)s;
                try
                {
                    await _appStateService.TaskProvider.UpdateTaskAsync(t.TaskId, t.IsComplete);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating Task {t.TaskId}: {ex}");
                }
            };
            _toDoList.Add(newTask);
        }
        public async Task RemoveTask(ToDoItem item)
        {
            if (!string.IsNullOrEmpty(item.EventId))
            {
                await _appStateService.TaskProvider.DeleteEventAsync(item.EventId);
            }
            await _appStateService.TaskProvider.DeleteTaskAsync(item.TaskId);
            await _localDBService.DeleteTask(item);
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
                        await _appStateService.TaskProvider.DeleteEventAsync(ToDoList[i].EventId);
                    }
                    await _appStateService.TaskProvider.DeleteTaskAsync(ToDoList[i].TaskId);
                    _toDoList.RemoveAt(i);
                }
            }
        }
        public void ClearTasks()
        {
            _toDoList.Clear();
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
            CompletedTasks = TotalTasks > 0 ? ToDoList.Count(item => item.IsComplete) * 100 / TotalTasks : 0;
        }
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}

