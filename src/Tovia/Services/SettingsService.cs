using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;
using Tovia.Bootstrap;
using Tovia.Models;

namespace Tovia.Services
{
    public class SettingsService
    {

        private readonly string _userSettingsPath;

        public event EventHandler SettingsChanged;

        public SettingsService()
        {
            _userSettingsPath = AppDataBootstrapper.AppUserSettings;
        }

        public void Save(UserSettings newUserSettings)
        {
            var json = JsonConvert.SerializeObject(newUserSettings, Formatting.Indented);
            File.WriteAllText(_userSettingsPath, json);
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
