using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tovia.Data;

namespace Tovia.interfaces
{
    public interface ILocalDBService
    {
        Task<List<ToDoItem>> GetTasksAsync();
        Task AddTaskAsync(UsersTasks task);
        Task DeleteTaskAsync(UsersTasks task);
    }
}
