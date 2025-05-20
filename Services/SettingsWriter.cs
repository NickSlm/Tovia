using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            jObject["Hotkeys"]["ModifierKey"] = newSettings.Hotkeys.ModifierKey.ToString();
            jObject["Hotkeys"]["MainKey"] = newSettings.Hotkeys.MainKey.ToString();

            jObject["WindowPosition"]["TopPos"] = newSettings.Window.TopPos.ToString();
            jObject["WindowPosition"]["LeftPos"] = newSettings.Window.LeftPos.ToString();


            //Dev Env
            File.WriteAllText("D:/Projects/ToDoListPlus/Config/appsettings.json", jObject.ToString(Formatting.Indented));

            // Save the updated JSON back to the file
            //File.WriteAllText(path, jObject.ToString(Formatting.Indented));

        }
    }
}
