using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;
using Tovia.Models;

namespace Tovia.Services
{
    public class SettingsService
    {

        private readonly string appSettingsPath;
        private readonly string userSettingsPath;
        private readonly string packagedUserSettings;

        public event EventHandler SettingsChanged;

        public SettingsService()
        {
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(localFolder, "Tovia");
            Directory.CreateDirectory(appFolder);

            userSettingsPath = Path.Combine(appFolder, "userSettings.json");

            var baseDir = AppContext.BaseDirectory;
            packagedUserSettings = Path.Combine(baseDir, "Config","usersettings.json");
            appSettingsPath = Path.Combine(baseDir, "Config", "appsettings.json");

            Load();
        }

        public UserSettings userSettings {get; private set;}
        public AppSettings appSettings { get; private set; }
        
        public void Load()
        {

            appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(appSettingsPath));

            if (!File.Exists(userSettingsPath))
            {
                if (File.Exists(packagedUserSettings))
                {
                    File.Copy(packagedUserSettings, userSettingsPath, overwrite:false);
                }
                else
                {
                    File.WriteAllText(userSettingsPath, JsonConvert.SerializeObject(new UserSettings(), Formatting.Indented));
                }
            }

            userSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(userSettingsPath));
        }
        public void Save(UserSettings newUserSettings)
        {
            var json = JsonConvert.SerializeObject(newUserSettings, Formatting.Indented);
            File.WriteAllText(userSettingsPath, json);
            userSettings = newUserSettings;
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
