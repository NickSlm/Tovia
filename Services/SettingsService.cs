using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class SettingsService
    {

        private const string SettingsPath = "Config/appsettings.json";

        public UserSettings Load()
        {
           var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(SettingsPath, optional: true, reloadOnChange: true)
            .Build();


            return configuration.Get<UserSettings>();
        }

        public void Save(UserSettings userSettings)
        {
            var path = Path.Combine(AppContext.BaseDirectory, SettingsPath);
            var json = File.ReadAllText(path);
            var jObject = JObject.Parse(json);


            jObject["Hotkeys"]["Overlay"]["MainKey"] = userSettings.Hotkeys["Overlay"].MainKey.ToString();
            jObject["Hotkeys"]["Overlay"]["ModifierKey"] = userSettings.Hotkeys["Overlay"].ModifierKey.ToString();

            jObject["Hotkeys"]["NewTask"]["MainKey"] = userSettings.Hotkeys["NewTask"].MainKey.ToString();
            jObject["Hotkeys"]["NewTask"]["ModifierKey"] = userSettings.Hotkeys["NewTask"].ModifierKey.ToString();

            jObject["Window"]["TopPos"] = userSettings.Window.TopPos.ToString();
            jObject["Window"]["LeftPos"] = userSettings.Window.LeftPos.ToString();

            jObject["Theme"]["BaseTheme"] = userSettings.Theme.BaseTheme.ToString();
            jObject["Theme"]["PrimaryColor"] = userSettings.Theme.PrimaryColor.ToString();
            jObject["Theme"]["SecondaryColor"] = userSettings.Theme.SecondaryColor.ToString();

            //Dev Env
            File.WriteAllText(path, jObject.ToString(Formatting.Indented));

        }

    }
}
