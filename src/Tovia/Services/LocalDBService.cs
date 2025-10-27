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

        public LocalDBService()
        {
            MessageBox.Show("LocalDBService init");
        }

        public async Task InitializeAsync()
        {
            using var db = new AppDbContext();
            await db.Database.EnsureCreatedAsync();
        }
        public async Task<List<ToDoItem>> GetTasksAsync()
        {
            throw new NotImplementedException();
        }
        public async Task AddTaskAsync(string title, string? description, DateTime? dateTime, string priority)
        {
            using var db = new AppDbContext();
        }
        public async Task DeleteTaskAsync(string taskId)
        {
            using var db = new AppDbContext();
        }
        public async Task UpdateTaskAsync(string taskId, bool IsComplete)
        {
            using var db = new AppDbContext();
        }


    }
}
