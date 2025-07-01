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

        public IConfiguration Load()
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(AppSettingsPath, optional:true, reloadOnChange:true)
            .AddJsonFile(UserSettingsPath, optional: true, reloadOnChange: true)
            .Build();

            return configuration;
        }

        public void Save(UserSettings userSettings)
        {
            var path = Path.Combine(AppContext.BaseDirectory, UserSettingsPath);
            var json = File.ReadAllText(path);
            var jObject = JObject.Parse(json);


            jObject["Hotkeys"]["Overlay"]["MainKey"] = userSettings.Hotkeys["Overlay"].MainKey.ToString();
            jObject["Hotkeys"]["Overlay"]["ModifierKey"] = userSettings.Hotkeys["Overlay"].ModifierKey.ToString();

            jObject["Hotkeys"]["NewTask"]["MainKey"] = userSettings.Hotkeys["NewTask"].MainKey.ToString();
            jObject["Hotkeys"]["NewTask"]["ModifierKey"] = userSettings.Hotkeys["NewTask"].ModifierKey.ToString();

            jObject["Window"]["TopPos"] = userSettings.Window.TopPos.ToString();
            jObject["Window"]["LeftPos"] = userSettings.Window.LeftPos.ToString();

            jObject["Theme"]["BaseTheme"] = userSettings.Theme.BaseTheme.ToString();

            //Dev Env
            File.WriteAllText(path, jObject.ToString(Formatting.Indented));

        }

    }
}
