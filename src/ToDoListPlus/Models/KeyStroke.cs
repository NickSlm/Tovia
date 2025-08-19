using System.ComponentModel;

namespace ToDoListPlus.Models
{
    public class KeyStroke: INotifyPropertyChanged
    {
        private string _keyStroke;

        public event PropertyChangedEventHandler? PropertyChanged;

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
