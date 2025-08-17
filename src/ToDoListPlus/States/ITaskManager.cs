using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListPlus.States
{
    public interface ITaskManager: INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<ToDoItem> ToDoList { get; }
        int TotalTasks { get; }
        int CompletedTasks { get; }
        int RemoveThis { get; }
        Task LoadToDoItems();
        Task RemoveTask(ToDoItem item);
        Task RemoveCompleteTask();
        Task SaveTask(ToDoItem item, bool createEvent);
        void ClearTasks();
    }
}
