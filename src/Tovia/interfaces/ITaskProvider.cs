using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovia.interfaces
{
    public interface ITaskProvider: INotifyPropertyChanged
    {
        Task<List<ToDoItem>> GetTasksAsync();
        Task<ToDoItem> CreateTaskAsync(ToDoItem item, bool createEvent);
        Task DeleteTaskAsync(string taskId);
        Task DeleteEventAsync(string eventId);
    }
}
