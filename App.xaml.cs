using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using ToDoListPlus.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ToDoListPlus.ViewModels;
using Microsoft.EntityFrameworkCore;
using ToDoListPlus.Views;
using ToDoListPlus.Models;
using System;
namespace ToDoListPlus;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    private AuthConfig _authConfig;

    protected override void OnStartup(StartupEventArgs e)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Config/authsettings.json");
        _authConfig = ConfigLoader.Load(path);

        var serviceCollection = new ServiceCollection();
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;


        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        ConfigureServices(serviceCollection, configuration);
        Services = serviceCollection.BuildServiceProvider();

        var globalHotKeyService = Services.GetRequiredService<GlobalHotKeyService>();
        globalHotKeyService.OnOverlayHotKeyPressed += () =>
        {
            var overlayWindow = Services.GetRequiredService<OverlayWindow>();
            overlayWindow.Show();
        };


        var loginWindow = Services.GetRequiredService<AuthorizationWindow>();
        bool? loginResult = loginWindow.ShowDialog();

        if (loginResult == true)
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
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

    private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HotkeySettings>(configuration.GetSection("Hotkeys"));

        services.AddSingleton<IAppStateResetService, AppStateResetService>();
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<AppStateService>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<TaskService>();
        services.AddSingleton<AuthConfig>(_authConfig);
        services.AddSingleton<GlobalHotKeyService>();

        services.AddSingleton<AuthorizationViewModel>();
        services.AddSingleton<ToDoListViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<NewTaskViewModel>();

        services.AddSingleton<SettingsView>();
        services.AddTransient<AuthorizationWindow>();
        services.AddTransient<OverlayWindow>();
        services.AddTransient<MainWindow>();
    }
}

