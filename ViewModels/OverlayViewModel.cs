using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ToDoListPlus.Models;
using ToDoListPlus.Services;
using ToDoListPlus.States;

namespace ToDoListPlus.ViewModels
{
    public class OverlayViewModel: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly TaskManager _taskManager;
        private double _topPos { get; set; }
        private double _leftPos { get; set; }
        private readonly SettingsService _settingsService;

        public ReadOnlyObservableCollection<ToDoItem> ToDoList => _taskManager.ToDoList;
        public OverlayPosition position { get; set; }
        public double TopPos
        {
            get => _topPos;
            set
            {
                _topPos = value;
                OnPropertyChanged(nameof(TopPos));
            }
        }
        public double LeftPos
        {
            get => _leftPos;
            set
            {
                _leftPos = value;
                OnPropertyChanged(nameof(LeftPos));
            }
        }

        public OverlayViewModel(SettingsService settingsService, TaskManager taskManager)
        {

            _settingsService = settingsService;
            _taskManager = taskManager;
            _settingsService.Load();

            var userSettings = _settingsService.userSettings;


            TopPos = userSettings.Window.TopPos;
            LeftPos = userSettings.Window.LeftPos;
        }

        public void UpdatePosition(double top, double left)
        {
            TopPos = top;
            LeftPos = left;
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
