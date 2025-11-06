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


        public async Task InitializeAsync()
        {
            using var db = new AppDbContext();
            await db.Database.EnsureCreatedAsync();
        }
        public async Task<List<ToDoItem>> GetTasksAsync()
        {
            throw new NotImplementedException();
        }
        public async Task AddTaskAsync(ToDoItem item)
        {
            using var db = new AppDbContext();
            await db.ToDoItems.AddAsync(item);
            await db.SaveChangesAsync();
        }
        public async Task DeleteTaskAsync(ToDoItem item)
        {
            using var db = new AppDbContext();
            db.ToDoItems.Remove(item);
            await db.SaveChangesAsync();
        }
        public async Task UpdateTaskAsync(int id, bool isComplete)
        {
            using var db = new AppDbContext();
            var task = await db.ToDoItems.FindAsync(id);

            if (task != null)
            {
                task.IsComplete = isComplete;
                await db.SaveChangesAsync();
            }
        }

    }
}
