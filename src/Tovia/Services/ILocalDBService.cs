using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovia.Services
{
    public interface ILocalDBService
    {
        Task<List<ToDoItem>> GetTasksAsync();
        Task AddTaskAsync(string title, string? description, DateTime? dateTime, string priority);
        Task DeleteTaskAsync(string taskId);
        Task UpdateTaskAsync(string taskId, bool isComplete);
    }
}
