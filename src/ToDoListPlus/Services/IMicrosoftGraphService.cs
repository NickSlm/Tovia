using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListPlus.Services
{
    public interface IMicrosoftGraphService: INotifyPropertyChanged
    {
        Task<ToDoItem> CreateTaskAsync(ToDoItem item, bool createEvent);
        Task<string> DeleteTaskAsync(string taskId);
        Task<string> UpdateTaskAsync(string taskId, bool IsComplete);
        Task<List<ToDoItem>> GetTasksAsync();
        Task<string> PostEventAsync(string title, string? description, DateTime? dateTime, string priority);
        Task<string> DeleteEventAsync(string eventId);
    }
}
