using Microsoft.Extensions.DependencyInjection;
using ToDoListPlus.Views;

namespace ToDoListPlus.Services
{
    public class DialogService: IDialogService
    {
        private readonly IServiceProvider _serviceProvider;


        public DialogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }
        public bool? ShowLoginDialog()
        {
            var loginWindow = _serviceProvider.GetRequiredService<AuthorizationWindow>();
            return loginWindow.ShowDialog();
        }

    }
}
