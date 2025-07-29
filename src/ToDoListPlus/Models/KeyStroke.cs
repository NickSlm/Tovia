using System.ComponentModel;

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
