using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListPlus.Services;

namespace ToDoListPlus.ViewModels
{

    public class MainViewModel
    {
        public ToDoListViewModel? ToDoListVM { get;  }

        public MainViewModel()
        {
            var dialogService = new DialogService();
            ToDoListVM = new ToDoListViewModel(dialogService);
        }
    }
}
