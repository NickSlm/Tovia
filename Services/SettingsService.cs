using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class SettingsService
    {

        private const string AppSettingsPath = "Config/appsettings.json";
        private const string UserSettingsPath = "Config/usersettings.json";

        public UserSettings userSettings { get; private set; }
        public AppSettings appSettings { get; private set; }

        public void Load()
        {
            var userPath = Path.Combine(AppContext.BaseDirectory, UserSettingsPath);
            var appPath = Path.Combine(AppContext.BaseDirectory, AppSettingsPath);

            userSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(userPath));
            appSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(appPath));
        }

        public void Save(UserSettings userSettings)
        {
            var path = Path.Combine(AppContext.BaseDirectory, UserSettingsPath);

            var json = JsonConvert.SerializeObject(userSettings, Formatting.Indented);

            File.WriteAllText(path, json);
        }
    }
}
