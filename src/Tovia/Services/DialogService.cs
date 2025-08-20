using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Tovia.Views;

namespace Tovia.Services
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

        public void ShowMessage(string message, string title = "Warning")
        {
            MessageBox.Show(message, title);
        }

    }
}
