using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tovia.interfaces;

namespace Tovia.Services
{
    public class GoogleApiService: ITaskProvider
    {
        public GoogleApiService()
        {

        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task<List<ToDoItem>> GetTasksAsync()
        {
            MessageBox.Show("Google get");
            return new List<ToDoItem>();
        }

        public async Task<ToDoItem> CreateTaskAsync(ToDoItem item, bool createEvent)
        {
            return item;
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
