using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToDoListPlus.Models;
using ToDoListPlus.Services;

namespace ToDoListPlus.ViewModels
{
    public class OverlayViewModel: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly TaskService _taskService;
        private double _topPos { get; set; }
        private double _leftPos { get; set; }
        private readonly SettingsService _settingsService;

        public ObservableCollection<ToDoItem> ToDoList => _taskService.ToDoList;
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

        public OverlayViewModel(SettingsService settingsService, TaskService taskService)
        {

            _settingsService = settingsService;
            _taskService = taskService;

            var conf = _settingsService.Load();
            var settings = conf.Get<UserSettings>();


            TopPos = settings.Window.TopPos;
            LeftPos = settings.Window.LeftPos;
        }

        public void UpdatePosition(double top, double left)
        {
            TopPos = top;
            LeftPos = left;
        }

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
