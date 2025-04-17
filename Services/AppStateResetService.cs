using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListPlus.ViewModels;

namespace ToDoListPlus.Services
{
    public class AppStateResetService: IAppStateResetService
    {
        private readonly ToDoListViewModel _toDoListViewModel;

        public AppStateResetService(ToDoListViewModel toDoListViewModel)
        {
            _toDoListViewModel = toDoListViewModel;
        }

        public void ResetState()
        {
            _toDoListViewModel.Reset();
        }
    }
}
