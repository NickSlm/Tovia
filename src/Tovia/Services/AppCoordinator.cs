using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tovia.States;

namespace Tovia.Services
{
    public class AppCoordinator
    {
        private readonly AppStateService _appStateService;
        private readonly ITaskManager _taskManager;

        public AppCoordinator(AppStateService appStateService, ITaskManager taskManager)
        {
            _appStateService = appStateService;
            _taskManager = taskManager;

            _appStateService.UserLoggedIn += OnUserLoggedIn;
            _appStateService.UserLoggedOut += OnUserLogOut;
        }

        public void OnUserLoggedIn()
        {
            _taskManager.LoadToDoItems();
        }
        public void OnUserLogOut()
        {
            _taskManager.ClearTasks();
        }

    }
}
