using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListPlus.Models
{
    public class UserSettings
    {
        public Dictionary<string, HotkeySettings> Hotkeys { get; set; } = new();
        public WindowSettings Window { get; set; }
        public ThemeSettings Theme { get; set; }
    }
}
