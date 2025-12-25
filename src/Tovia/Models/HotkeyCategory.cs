using System.Collections;
using System.Collections.Generic;

namespace Tovia.Models
{
    public class HotkeyCategory: IEnumerable<KeyValuePair<string, HotkeySettings>>
    {

        public HotkeySettings Overlay { get; set; }
        public HotkeySettings Settings { get; set; }

        public IEnumerator<KeyValuePair<string, HotkeySettings>> GetEnumerator()
        {
            if (Overlay != null) yield return new KeyValuePair<string, HotkeySettings>("Overlay", Overlay);
            if (Settings != null) yield return new KeyValuePair<string, HotkeySettings>("Settings", Settings);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
