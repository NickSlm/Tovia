using System.Collections.ObjectModel;
using System.Timers;
using ToDoListPlus.Models;


namespace ToDoListPlus.Services
{
    public class AppTimerService
    {

        private readonly TaskService _taskService;
        private ObservableCollection<ToDoItem> ToDoList => _taskService.ToDoList;


        public System.Timers.Timer aTimer;
        public AppTimerService(TaskService taskService)
        {
            _taskService = taskService;
            SetTimer();
        }
        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
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
                    item.Status = TaskState.Failed;
                    item.TimeLeft = TimeSpan.Zero;
                    continue;
                }
                item.TimeLeft = item.DueDate - DateTime.Now;
            }
        }
    }
}
