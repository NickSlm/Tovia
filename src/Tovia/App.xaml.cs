using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Tovia.Views;
using Tovia.Services;
using Tovia.States;
using Tovia.ViewModels;
using Tovia.interfaces;
using Tovia.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
namespace Tovia;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    private static readonly string AppFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "Tovia");
    private static readonly string DatabasePath = Path.Combine(AppFolder, "Tovia.db");

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        if (!Directory.Exists(AppFolder))
            Directory.CreateDirectory(AppFolder);

        using (var scope = Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<LocalDbContext>>();
            var db = factory.CreateDbContext();
            db.Database.Migrate();
        }

        _ = Services.GetRequiredService<AppThemeService>();

        var loginWindow = Services.GetRequiredService<AuthorizationWindow>();
        bool? loginResult = loginWindow.ShowDialog();


        if (loginResult == false)
        {
            Application.Current.Shutdown();
            return;
        }

        _ = Services.GetRequiredService<AppCoordinator>();
        _ = Services.GetRequiredService<AppTimerService>();
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
        services.AddDbContextFactory<LocalDbContext>(options =>
                     options.UseSqlite($"Data Source={DatabasePath}"));

        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<ITaskManager, TaskManager>();
        services.AddSingleton<IMicrosoftGraphService, MicrosoftGraphService>();
        services.AddTransient<ISyncDBService, SyncDBService>();
        services.AddScoped<ILocalDBService, LocalDBService>();

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

