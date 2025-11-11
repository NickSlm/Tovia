using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tovia.Data;
using Tovia.interfaces;

namespace Tovia.Services
{
    public class LocalDBService:ILocalDBService
    {
        private readonly AppDbContext _dbContext;


        public LocalDBService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ToDoItem>> GetTasksAsync()
        {
            throw new NotImplementedException();
        }
        public async Task AddTaskAsync(ToDoItem item)
        {

            await _dbContext.ToDoItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteTaskAsync(ToDoItem item)
        {
            _dbContext.ToDoItems.Remove(item);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateTaskAsync(int id, bool isComplete)
        {
            var task = await _dbContext.ToDoItems.FindAsync(id);

            if (task != null)
            {
                task.IsComplete = isComplete;
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
