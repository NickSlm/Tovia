using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tovia.Data
{
    public class AppDbContext: DbContext
    {
        private readonly string _databasePath;
        private readonly string _localFolder;
        private readonly string _appFolder;

        public AppDbContext()
        {
            _localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _appFolder = Path.Combine(_localFolder, "Tovia");
            _databasePath = Path.Combine(_appFolder, "Tovia.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
                options.UseSqlite($"Data Source={_databasePath}");
        }


    }
}
