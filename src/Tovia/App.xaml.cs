using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Tovia.Views;
using Tovia.Services;
using Tovia.States;
using Tovia.ViewModels;
using Tovia.interfaces;
namespace Tovia;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        _ = Services.GetRequiredService<AppThemeService>();

        var loginWindow = Services.GetRequiredService<AuthorizationWindow>();
        bool? loginResult = loginWindow.ShowDialog();


        if (loginResult == false)
        {
            Application.Current.Shutdown();
            return;
        }
        var dbService = Services.GetRequiredService<ILocalDBService>();
        await dbService.InitializeAsync();
        _ = Services.GetRequiredService<AppTimerService>();
        _ = Services.GetRequiredService<AppCoordinator>();
        var globalHotKeyService = Services.GetRequiredService<GlobalHotKeyService>();
        var overlayWindow = Services.GetRequiredService<OverlayWindow>();
        var mainWindow = Services.GetRequiredService<MainWindow>();

        globalHotKeyService.OnOverlayHotKeyPressed += () => ToggleOverlay(overlayWindow);        
        Application.Current.MainWindow = mainWindow;
        mainWindow.Show();

    }
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }
    private void ToggleOverlay(OverlayWindow overlayWindow)
    {
        overlayWindow.DataContext = Services.GetRequiredService<OverlayViewModel>();

        if (overlayWindow.IsVisible) { overlayWindow.Hide(); }
        else
        {
            var vm = (OverlayViewModel)overlayWindow.DataContext;

            overlayWindow.Top = vm.TopPos;
            overlayWindow.Left = vm.LeftPos;

            overlayWindow.Show();
            overlayWindow.Activate();
        }
    }
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<ITaskManager, TaskManager>();
        services.AddSingleton<IMicrosoftGraphService, MicrosoftGraphService>();
        services.AddSingleton<ILocalDBService, LocalDBService>();

        services.AddSingleton<AppStateService>();
        services.AddSingleton<AppCoordinator>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<AppTimerService>();
        services.AddSingleton<GlobalHotKeyService>();
        services.AddSingleton<AppThemeService>();
        services.AddSingleton<SettingsService>();

        services.AddSingleton<AuthorizationViewModel>();
        services.AddSingleton<ToDoListViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<OverlayViewModel>();
        services.AddTransient<SettingsViewModel>();

        services.AddTransient<SettingsView>();
        services.AddTransient<AuthorizationWindow>();
        services.AddTransient<OverlayWindow>();
        services.AddTransient<MainWindow>();
    }
}

