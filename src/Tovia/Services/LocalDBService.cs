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
        private readonly LocalDbContext _dbContext;
        private readonly AuthService _authService;

        public LocalDBService(LocalDbContext dbContext, AuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }

        public async Task SaveTask(UsersTasks task)
        {
            var user = await _dbContext.Users.Include(e => e.Tasks)
                .FirstOrDefaultAsync(u => u.MicrosoftOid == _authService.OID);

            if (user == null)
            {
                user = new Users()
                {
                    AccountName = _authService.AccountUsername,
                    MicrosoftOid = _authService.OID,
                    LastLogin = DateTime.Now,
                };

                _dbContext.Users.Add(user);
            }
            user.Tasks.Add(task);

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTask(ToDoItem item)
        {
            var user = await _dbContext.Users.Include(e => e.Tasks).
                FirstOrDefaultAsync(u => u.MicrosoftOid == _authService.OID);

            var task = user.Tasks.FirstOrDefault(t => t.TaskId == item.TaskId);

            user.Tasks.Remove(task);

            await _dbContext.SaveChangesAsync();
        }
    }
}
