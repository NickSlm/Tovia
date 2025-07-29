namespace ToDoListPlus.Models
{
    public class UserSettings
    {
        public WindowSettings Window { get; set; }
        public AppearanceSettings Appearance { get; set; }
        public Dictionary<string, HotkeySettings> Hotkeys { get; set; } = new();
    }
}
