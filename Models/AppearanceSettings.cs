namespace ToDoListPlus.Models
{
    public class AppearanceSettings
    {
        public string BaseTheme { get; set; } = "light";
        public string FailedTask { get; set; }
        public string InProgressTask { get; set; }
        public string CompleteTask { get; set; }
    }
}
