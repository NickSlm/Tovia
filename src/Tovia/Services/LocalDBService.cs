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
        private readonly IDbContextFactory<AppDbContext> _dbFactory;


        public LocalDBService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task InitializeAsync()
        {
            await using var _db = _dbFactory.CreateDbContext();
            await _db.Database.EnsureCreatedAsync();
        }
        public async Task<List<ToDoItem>> GetTasksAsync()
        {
            throw new NotImplementedException();
        }
        public async Task AddTaskAsync(ToDoItem item)
        {
            await using var _db = _dbFactory.CreateDbContext();

            await _db.ToDoItems.AddAsync(item);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteTaskAsync(ToDoItem item)
        {
            await using var _db = _dbFactory.CreateDbContext();

            _db.ToDoItems.Remove(item);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateTaskAsync(int id, bool isComplete)
        {
            await using var _db = _dbFactory.CreateDbContext();

            var task = await _db.ToDoItems.FindAsync(id);

            if (task != null)
            {
                task.IsComplete = isComplete;
                await _db.SaveChangesAsync();
            }
        }

    }
}
