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

        public MainViewModel(Window parentWindow)
        {

            var dialogService = new DialogService();
            ToDoListVM = new ToDoListViewModel(dialogService);
            AuthorizationVM = new AuthorizationViewModel(parentWindow);
        }
    }
}
