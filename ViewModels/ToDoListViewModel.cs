using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ToDoListPlus.Services;
using ToDoListPlus.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Timers;
using ToDoListPlus.Models;
using CommunityToolkit.Mvvm.Input;


namespace ToDoListPlus.ViewModels
{
    public class ToDoListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
            
        public IAsyncRelayCommand<ObservableCollection<ToDoItem>> CleanUpCommand { get; }
        public IAsyncRelayCommand<ToDoItem> RemoveTaskCommand { get; }
        public IRelayCommand<ToDoItem> ToggleReadOnlyCommand { get; }
        public  ObservableCollection<ToDoItem> ToDoList => _taskService.ToDoList;

        private static System.Timers.Timer aTimer;

        private readonly AuthService _authService;
        private readonly TaskService _taskService;
        private readonly AppStateService _appStateService;
        private int _completedTasks;
        private int _totalTasks;

        public int TotalTasks
        {
            get => _totalTasks;
            set
            {
                _totalTasks = value;
                OnPropertyChanged(nameof(TotalTasks));
            }
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
        public ToDoListViewModel(AuthService authService, TaskService taskService, AppStateService appStateService)
        {
            _authService = authService;
            _taskService = taskService;
            _appStateService = appStateService;

            _appStateService.UserLoggedIn += OnUserLoggedIn;

            CleanUpCommand = new AsyncRelayCommand<ObservableCollection<ToDoItem>>(CleanCompletedItems);
            RemoveTaskCommand = new AsyncRelayCommand<ToDoItem>(RemoveItem);
            ToggleReadOnlyCommand = new RelayCommand<ToDoItem>(ToggleReadOnly);

            _taskService.ToDoList.CollectionChanged += (s, e) => HandleCollectionChanged(e);
            _taskService.ToDoList.CollectionChanged += (s, e) => UpdateTotalTasks();

            UpdateCompletedTasks();
        }

        public void OnUserLoggedIn()
        {
            LoadToDoItems();
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
            UpdateCompletedTasks();
        }
        private void UpdateCompletedTasks()
        {
            CompletedTasks = (TotalTasks > 0) ? (ToDoList.Count(item => item.IsComplete) * 100) / TotalTasks : 0;
        }
        public async void LoadToDoItems()
        {
            var tasks = await _taskService.GetTasksAsync();

            ToDoList.Clear();
            foreach (var task in tasks)
            {
                task.OnCompletionChanged += async (s, e) =>
                {
                    var t = (ToDoItem)s;
                    try
                    {
                        await _taskService.UpdateTaskAsync(t.TaskId, t.IsComplete);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }; 
                _taskService.ToDoList.Add(task);
            }

            UpdateCompletedTasks();
            UpdateTotalTasks();
        }
        private void UpdateTotalTasks()
        {
            TotalTasks = _taskService.ToDoList.Count;
        }
        private async void ToggleReadOnly(ToDoItem item)
        {
            item.IsReadOnly = !item.IsReadOnly;
        }
        private async Task RemoveItem(ToDoItem item)
        {
            if (!string.IsNullOrEmpty(item.EventId))
            {
                string eventResult = await _taskService.DeleteEventAsync(item.EventId);
                MessageBox.Show(eventResult);
            }
            _taskService.ToDoList.Remove(item);
            string taskResult = await _taskService.DeleteTaskAsync(item.TaskId);
            MessageBox.Show(taskResult);
        }
        private async Task CleanCompletedItems(ObservableCollection<ToDoItem> toDoList)
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
                    ToDoList.RemoveAt(i);
                }
            }
        }
        public void Reset()
        {
            _taskService.ToDoList.Clear();
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}