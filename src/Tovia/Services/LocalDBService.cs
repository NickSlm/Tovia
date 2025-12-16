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
        public async Task AddTaskAsync(Item item)
        {

            await _dbContext.Tasks.AddAsync(item);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteTaskAsync(Item item)
        {
            _dbContext.Tasks.Remove(item);
            await _dbContext.SaveChangesAsync();
        }
    }
}
