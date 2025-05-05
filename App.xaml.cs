using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using ToDoListPlus.Services;
using Microsoft.Extensions.DependencyInjection;
using ToDoListPlus.ViewModels;
using Microsoft.EntityFrameworkCore;
using ToDoListPlus.Views;
using ToDoListPlus.Models;
namespace ToDoListPlus;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider _serviceProvider;
    private AuthConfig _authConfig;

    protected override void OnStartup(StartupEventArgs e)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Config/authsettings.json");
        _authConfig = ConfigLoader.Load(path);

        var serviceCollection = new ServiceCollection();
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

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

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IAppStateResetService, AppStateResetService>();
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<AppStateService>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<TaskService>();
        services.AddSingleton<AuthConfig>(_authConfig);

        services.AddSingleton<AuthorizationViewModel>();
        services.AddSingleton<ToDoListViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddTransient<AuthorizationWindow>();
        services.AddTransient<MainWindow>();
    }
}

