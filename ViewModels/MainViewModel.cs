using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToDoListPlus.Services;

namespace ToDoListPlus.ViewModels
{

    public class MainViewModel
    {
        public ToDoListViewModel? ToDoListVM { get;  }
        public AuthorizationViewModel? AuthorizationVM { get; }

        public MainViewModel(ToDoListViewModel toDoListVM, AuthorizationViewModel? authorizationVM)
        {
            ToDoListVM = toDoListVM;
            AuthorizationVM = authorizationVM;
        }
    }
}
