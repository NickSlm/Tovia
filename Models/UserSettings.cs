namespace ToDoListPlus.Models
{
    public class UserSettings
    {
        public WindowSettings Window { get; set; }
        public ThemeSettings Theme { get; set; }
        public Dictionary<string, HotkeySettings> Hotkeys { get; set; } = new();
    }
}
