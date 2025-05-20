using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDoListPlus.Services;

namespace ToDoListPlus.ViewModels
{
    public class NewTaskViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly DelegateCommand _saveTaskCommand;
        private readonly TaskService _taskService;
        private string _taskTitle = string.Empty;
        private string _taskDescription = string.Empty;
        private DateTime? _taskDueDate = DateTime.Now;
        private bool _eventIsChecked = false;
        private string _taskImportance = string.Empty;

        public ICommand SaveTaskCommand => _saveTaskCommand;
        public string TaskTitle
        {
            get => _taskTitle;
            set
            {
                _taskTitle = value;
                OnPropertyChanged(nameof(TaskTitle));
            }
        }
        public string? TaskDescription
        {
            get => _taskDescription;
            set
            {
                _taskDescription = value;
                OnPropertyChanged(nameof(TaskDescription));
            }
        }
        public DateTime? TaskDueDate
        {
            get => _taskDueDate;
            set
            {
                _taskDueDate = value;
                OnPropertyChanged(nameof(TaskDueDate));
            }
        }
        public bool EventIsChecked
        {
            get => _eventIsChecked;
            set
            {
                _eventIsChecked = value;
                OnPropertyChanged(nameof(EventIsChecked));
            }
        }
        public string TaskImportance
        {
            get => _taskImportance;
            set
            {
                if (_taskImportance != null)
                {
                    _taskImportance = value;
                    OnPropertyChanged(nameof(TaskImportance));
                }
            }
        }

        public NewTaskViewModel(TaskService taskService)
        {
            _taskService = taskService;
            _saveTaskCommand = new DelegateCommand(SaveTask, CanExecute);
        }

        private async void SaveTask(object commandParameter)
        {
            if (string.IsNullOrWhiteSpace(TaskTitle))
            {
                MessageBox.Show("Title Required");
                return;
            }
            if (!TaskDueDate.HasValue)
            {
                MessageBox.Show("DueTime Required");
                return;
            }
            if (string.IsNullOrEmpty(TaskImportance))
            {
                MessageBox.Show("Priority Required");
                return;
            }

            ToDoItem newTask = await _taskService.CreateTaskAsync(TaskTitle, TaskDescription, TaskDueDate, TaskImportance, EventIsChecked);
            var toDoListVM = App.Services.GetRequiredService<ToDoListViewModel>();
            toDoListVM.ToDoList.Add(newTask);

            //Reset Form Fields
            TaskTitle = string.Empty;
            TaskDescription = string.Empty;
            TaskDueDate = DateTime.Now;
            TaskImportance = string.Empty;
            EventIsChecked = false;

        }
        private bool CanExecute(object commandParameter)
        {
            return true;
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
