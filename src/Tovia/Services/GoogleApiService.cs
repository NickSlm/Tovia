using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using GoogleTask = Google.Apis.Tasks.v1.Data.Task;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tovia.interfaces;
using Tovia.Models;

namespace Tovia.Services
{
    public class GoogleApiService: ITaskProvider
    {
        private readonly TasksService _tasksService;
        private readonly string _accessToken;
        private readonly UserProfile _userProfile;

        public GoogleApiService(string accessToken, UserProfile user, UserCredential credentials)
        {
            _accessToken = accessToken;
            _userProfile = user;
            _tasksService = new TasksService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = "Tovia"
            });
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task<List<ToDoItem>> GetTasksAsync()
        {
            //yield maybe?
            var response = await _tasksService.Tasks.List(_userProfile.TaskListId).ExecuteAsync();
            var tasks = response.Items;

            var taskList = new List<ToDoItem>();

            foreach (var task in tasks)
            {
                var item = new ToDoItem
                {
                    TaskId = task.Id,
                    Title = task.Title,
                    Description = task.Notes,
                    DueDate = DateTime.TryParse(task.Due, out var parsedDateTime) ? parsedDateTime : null,
                    IsComplete = task.Status == "completed" ? true : false,
                };
                taskList.Add(item);
            };

            return taskList;
        }
        public async Task<ToDoItem> CreateTaskAsync(ToDoItem item, bool createEvent)
        {
            var task = new GoogleTask()
            {
                Title = item.Title,
                Notes = item.Description,
                Due = item.DueDate.Value.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                Status = "needsAction",
            };
            var response = await _tasksService.Tasks.Insert(task, _userProfile.TaskListId).ExecuteAsync();

            item.TaskId = response.Id;

            return item;
        }
        public async Task UpdateTaskAsync(string taskId, bool isComplete)
        {
            var patchTask = new GoogleTask()
            {
                Status = isComplete ? "completed" : "needsAction",
                Completed = isComplete ? DateTime.UtcNow.ToString("o") : null
                
            };
            await _tasksService.Tasks.Patch(patchTask, _userProfile.TaskListId, taskId).ExecuteAsync();
        }
        public async Task DeleteTaskAsync(string taskId)
        {
            await _tasksService.Tasks.Delete(_userProfile.TaskListId, taskId).ExecuteAsync();
        }

        public async Task DeleteEventAsync(string eventId)
        {
            await Task.Delay(1000);
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
