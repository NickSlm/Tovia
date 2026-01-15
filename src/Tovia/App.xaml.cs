using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Windows;
using Tovia.Views;
using Tovia.Services;
using Tovia.States;
using Tovia.ViewModels;
using Tovia.interfaces;
using Tovia.Bootstrap;
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


    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        AppDataBootstrapper.EnsureAppDataInitialized();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(AppDataBootstrapper.AppUserSettings, optional: false, reloadOnChange: true)
            .Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        ConfigureServices(serviceCollection, configuration);
        Services = serviceCollection.BuildServiceProvider();

        using (var scope = Services.CreateScope())
        {
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<LocalDbContext>>();
            var db = factory.CreateDbContext();
            db.Database.Migrate();
        }

        _ = Services.GetRequiredService<AppCoordinator>();
        _ = Services.GetRequiredService<AppThemeService>();

        var loginWindow = Services.GetRequiredService<AuthorizationWindow>();
        bool? loginResult = loginWindow.ShowDialog();


        if (loginResult == false)
        {
            Application.Current.Shutdown();
            return;
        }

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
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<LocalDbContext>(options =>
                     options.UseSqlite($"Data Source={AppDataBootstrapper.AppDatabase}"));

        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<ITaskManager, TaskManager>();
        services.AddTransient<ISyncDBService, SyncDBService>();
        services.AddScoped<ILocalDBService, LocalDBService>();

        services.AddSingleton<AppStateService>();
        services.AddSingleton<AppCoordinator>();
        services.AddSingleton<MicrosoftAuthService>();
        services.AddSingleton<GoogleAuthService>();
        services.AddSingleton<AppTimerService>();
        services.AddSingleton<GlobalHotKeyService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<AppThemeService>();

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

