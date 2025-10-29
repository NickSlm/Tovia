using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovia.interfaces
{
    public interface ILocalDBService
    {
        Task InitializeAsync();
        Task<List<ToDoItem>> GetTasksAsync();
        Task AddTaskAsync(ToDoItem item);
        Task DeleteTaskAsync(ToDoItem item);
        Task UpdateTaskAsync(int id, bool isComplete);
    }
}
