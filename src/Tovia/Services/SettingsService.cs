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

        private readonly string _userSettingsPath;
        private readonly string _packagedUserSettings;

        public event EventHandler SettingsChanged;

        public SettingsService()
        {
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(localFolder, "Tovia");
            Directory.CreateDirectory(appFolder);

            _userSettingsPath = Path.Combine(appFolder, "userSettings.json");

            var baseDir = AppContext.BaseDirectory;
            _packagedUserSettings = Path.Combine(baseDir, "Config","usersettings.json");

            Load();
        }

        public UserSettings userSettings {get; private set;}
        public void Load()
        {
            if (!File.Exists(_userSettingsPath))
            {
                if (File.Exists(_packagedUserSettings))
                {
                    File.Copy(_packagedUserSettings, _userSettingsPath, overwrite:false);
                }
                else
                {
                    File.WriteAllText(_userSettingsPath, JsonConvert.SerializeObject(new UserSettings(), Formatting.Indented));
                }
            }
            userSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(_userSettingsPath));
        }
        public void Save(UserSettings newUserSettings)
        {
            var json = JsonConvert.SerializeObject(newUserSettings, Formatting.Indented);
            File.WriteAllText(_userSettingsPath, json);
            userSettings = newUserSettings;
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
