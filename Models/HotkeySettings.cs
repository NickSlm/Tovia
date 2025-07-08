using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Windows.Input;
using ToDoListPlus.Converters;

namespace ToDoListPlus.Models
{
    public class HotkeySettings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ModifierKeys ModifierKey { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Key MainKey { get; set; }
    }
}
