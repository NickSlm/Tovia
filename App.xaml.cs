using System.Configuration;
using System.Data;
using System.Windows;
using ToDoListPlus.Services;
using Microsoft.Extensions.DependencyInjection;
using ToDoListPlus.ViewModels;
using Microsoft.EntityFrameworkCore;
using ToDoListPlus.Views;

namespace ToDoListPlus;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();


        var authService = _serviceProvider.GetRequiredService<AuthService>();


        if (!string.IsNullOrEmpty(authService.AccessToken))
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        }
        else
        {
            var loginWindow = _serviceProvider.GetRequiredService<AuthorizationWindow>();
            bool? loginResult = loginWindow.ShowDialog();

            if (loginResult == true)
            {
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {

        services.AddSingleton<IAppStateResetService, AppStateResetService>();
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<AuthService>();
        services.AddSingleton<TaskService>();

        services.AddSingleton<ToDoListViewModel>();
        services.AddSingleton<AuthorizationViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddTransient<AuthorizationWindow>();
        services.AddSingleton<MainWindow>();
    }
}

