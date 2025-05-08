using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListPlus.ViewModels
{
    public class SettingsViewModel: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsViewModel()
        {

        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
