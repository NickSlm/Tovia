﻿using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Tovia.interfaces;
using Tovia.Models;
using Tovia.Services;

namespace Tovia.ViewModels
{
    public class OverlayViewModel: INotifyPropertyChanged
    {
        private readonly SettingsService _settingsService;
        private readonly ITaskManager _taskManager;
        private double _topPos;
        private double _leftPos;
        private string _inProgressTaskColor;
        private string _failedTaskColor;
        private string _completedTaskColor;

        public event PropertyChangedEventHandler? PropertyChanged;

        public OverlayViewModel(SettingsService settingsService, ITaskManager taskManager)
        {

            _settingsService = settingsService;
            _taskManager = taskManager;

            _settingsService.SettingsChanged += (s, e) => ApplySettings();
            ApplySettings();
        }

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
        public string InProgressTaskColor
        {
            get => _inProgressTaskColor;
            set
            {
                _inProgressTaskColor = value;
                OnPropertyChanged(nameof(InProgressTaskColor));
            }
        }
        public string FailedTaskColor
        {
            get => _failedTaskColor;
            set
            {
                _failedTaskColor = value;
                OnPropertyChanged(nameof(FailedTaskColor));
            }
        }
        public string CompletedTaskColor
        {
            get => _completedTaskColor;
            set
            {
                _completedTaskColor = value;
                OnPropertyChanged(nameof(CompletedTaskColor));
            }
        }


        private void ApplySettings()
        {
            var userSettings = _settingsService.userSettings;

            TopPos = userSettings.Window.TopPos;
            LeftPos = userSettings.Window.LeftPos;

            InProgressTaskColor = userSettings.Appearance.InProgressTask;
            FailedTaskColor = userSettings.Appearance.FailedTask;
            CompletedTaskColor = userSettings.Appearance.CompleteTask;
        }
        public void UpdatePosition(double top, double left)
        {
            TopPos = top;
            LeftPos = left;
        }
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
