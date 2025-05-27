using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListPlus.Models
{
    public class KeyStroke: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _keyStroke;
        public string keyStroke
        {
            get => _keyStroke;
            set
            {
                _keyStroke = value;
                OnPropertyChanged(nameof(keyStroke));
            }
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
