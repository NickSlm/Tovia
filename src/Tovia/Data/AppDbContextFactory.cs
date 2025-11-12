using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovia.Data
{
    public class AppDbContextFactory: IDesignTimeDbContextFactory<AppDbContext>
    {
        /// <summary>
        /// Design-time factory for EF Core migrations in a WPF application.
        /// 
        /// EF Core tools (e.g., `dotnet ef migrations add`) need to instantiate
        /// the DbContext at design-time, before the WPF app is actually running.
        /// 
        /// In WPF, the DI container and OnStartup method are runtime-only,
        /// so EF Core cannot resolve DbContextOptions automatically.
        /// 
        /// This factory provides EF Core with a way to create AppDbContext
        /// independently of the runtime DI container, enabling migrations to work.
        /// </summary>
        public AppDbContext CreateDbContext(string[] args)
        {
            var appFolder = Path.Combine(
              Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
              "Tovia");

            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            var databasePath = Path.Combine(appFolder, "Tovia.db");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite($"Data Source={databasePath}");

            return new AppDbContext(optionsBuilder.Options);
        }

    }
}
