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
        public async Task AddTaskAsync(UsersTasks task)
        {
            await _dbContext.UsersTasks.AddAsync(task);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteTaskAsync(UsersTasks task)
        {
            _dbContext.UsersTasks.Remove(task);
            await _dbContext.SaveChangesAsync();
        }
    }
}
