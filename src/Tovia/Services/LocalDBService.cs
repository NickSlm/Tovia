using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovia.Services
{
    public class LocalDBService:ILocalDBService
    {

        //WIP Class


        public async Task<List<ToDoItem>> GetTasksAsync()
        {
            var taskList = new List<ToDoItem>();
            return taskList;
        }
        public async Task AddTaskAsync(string title, string? description, DateTime? dateTime, string priority)
        {

        }
        public async Task DeleteTaskAsync(string taskId)
        {

        }
        public async Task UpdateTaskAsync(string taskId, bool IsComplete)
        {

        }


    }
}
