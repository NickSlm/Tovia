using System.Collections.ObjectModel;
using System.Timers;
using System.Windows;
using Tovia.interfaces;
using Tovia.Models;


namespace Tovia.Services
{
    public class AppTimerService
    {

        private readonly ITaskManager _taskManager;
        public System.Timers.Timer aTimer;

        public AppTimerService(ITaskManager taskManager)
        {
            _taskManager = taskManager;
            SetTimer();
        }

        private ReadOnlyObservableCollection<ToDoItem> ToDoList 
        {
            get => _taskManager.ToDoList;
        } 

        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            for (int i = ToDoList.Count - 1; i >= 0; i--)
            {
                var item = ToDoList[i];

                if (item.Status == TaskState.Complete) 
                {
                    item.TimeLeft = TimeSpan.Zero;
                    continue;
                }
                if (DateTime.Now > item.DueDate)
                {
                    item.TimeLeft = TimeSpan.Zero;
                    continue;
                }
                item.TimeLeft = item.DueDate - DateTime.Now;
            }
        }
    }
}
