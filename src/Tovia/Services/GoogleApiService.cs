using Google.Apis.Services;
using Google.Apis.Tasks.v1;
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

        public GoogleApiService(string accessToken, UserProfile user)
        {
                _accessToken = accessToken;
                _userProfile = user;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task<List<ToDoItem>> GetTasksAsync()
        {

            return new List<ToDoItem>();
        }

        public async Task<ToDoItem> CreateTaskAsync(ToDoItem item, bool createEvent)
        {
            return item;
        }
        public async Task UpdateTaskAsync(string taskId, bool isComplete)
        {
            await Task.Delay(1000);

        }
        public async Task DeleteTaskAsync(string taskId)
        {
            await Task.Delay(1000);
        }

        public async Task DeleteEventAsync(string eventId)
        {
            await Task.Delay(1000);
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
