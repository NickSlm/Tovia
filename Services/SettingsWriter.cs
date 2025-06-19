using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class SettingsWriter
    {
        public static void UpdateSettings(UserSettings newSettings)
        {

            var path = Path.Combine(AppContext.BaseDirectory, "Config/appsettings.json");
            var json = File.ReadAllText(path);
            var jObject = JObject.Parse(json);


            jObject["Hotkeys"]["Overlay"]["MainKey"] = newSettings.Hotkeys["Overlay"].MainKey.ToString();
            jObject["Hotkeys"]["Overlay"]["ModifierKey"] = newSettings.Hotkeys["Overlay"].ModifierKey.ToString();

            jObject["Hotkeys"]["NewTask"]["MainKey"] = newSettings.Hotkeys["NewTask"].MainKey.ToString();
            jObject["Hotkeys"]["NewTask"]["ModifierKey"] = newSettings.Hotkeys["NewTask"].ModifierKey.ToString();

            jObject["WindowPosition"]["TopPos"] = newSettings.Window.TopPos.ToString();
            jObject["WindowPosition"]["LeftPos"] = newSettings.Window.LeftPos.ToString();

            jObject["Theme"]["BaseTheme"] = newSettings.Theme.BaseTheme.ToString();
            jObject["Theme"]["PrimaryColor"] = newSettings.Theme.PrimaryColor.ToString();
            jObject["Theme"]["SecondaryColor"] = newSettings.Theme.SecondaryColor.ToString();

            //Dev Env
            File.WriteAllText("D:/Projects/ToDoListPlus/Config/appsettings.json", jObject.ToString(Formatting.Indented));

            // Save the updated JSON back to the file
            //File.WriteAllText(path, jObject.ToString(Formatting.Indented));

        }
    }
}
