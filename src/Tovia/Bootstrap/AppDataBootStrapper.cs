using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tovia.Bootstrap
{
    public static class AppDataBootstrapper
    {
        public static string AppFolder { get; }
        public static string AppUserSettings { get; }
        public static string AppDatabase { get; }


        static AppDataBootstrapper()
        {
            //  AppData/Local/Tovia
            AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tovia");

            //  AppData/Local/Tovia.db
            AppDatabase = Path.Combine(AppFolder, "Tovia.db");

            //  AppData/Local/Tovia/usersettings.json
            AppUserSettings = Path.Combine(AppFolder, "usersettings.json");

        }

        public static void EnsureAppDataInitialized()
        {
            Directory.CreateDirectory(AppFolder);

            if (!File.Exists(AppUserSettings))
            {
                //  src/Tovia/Config/usersettings.json
                var template = Path.Combine(AppContext.BaseDirectory, "Config", "usersettings.json");

                File.Copy(template, AppUserSettings);
            }

        }
    }
}
