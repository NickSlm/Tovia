using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ToDoListPlus.Services;
using ToDoListPlus.States;
using ToDoListPlus.ViewModels;
using ToDoListPlus.Views;
namespace ToDoListPlus;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        var themeService = Services.GetRequiredService<AppThemeService>();
        themeService.InitializeTheme();

        var loginWindow = Services.GetRequiredService<AuthorizationWindow>();
        bool? loginResult = loginWindow.ShowDialog();

        if (loginResult != true)
        {
            Application.Current.Shutdown();
        }

        var globalHotKeyService = Services.GetRequiredService<GlobalHotKeyService>();
        var overlayWindow = Services.GetRequiredService<OverlayWindow>();
        var appTimerService = Services.GetRequiredService<AppTimerService>();
        var mainWindow = Services.GetRequiredService<MainWindow>();

        globalHotKeyService.OnOverlayHotKeyPressed += () => ToggleOverlay(overlayWindow);
        globalHotKeyService.OnNewTaskHotKeyPressed += () => ToggleNewTask();
        
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
    private async void ToggleNewTask()
    {
        var newTaskView = new NewTaskView();
        newTaskView.DataContext = Services.GetRequiredService<NewTaskViewModel>();
        var isDiagOpen = DialogHost.IsDialogOpen("RootDialog");
        if (!isDiagOpen)
        {
            var result = await DialogHost.Show(newTaskView, "RootDialog");
        }
    }
    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<AppStateService>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<MicrosoftGraphService>();
        services.AddSingleton<AppTimerService>();
        services.AddSingleton<GlobalHotKeyService>();
        services.AddSingleton<AppThemeService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<TaskManager>();

        services.AddSingleton<AuthorizationViewModel>();
        services.AddSingleton<ToDoListViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<OverlayViewModel>();
        services.AddSingleton<NewTaskViewModel>();
        services.AddTransient<SettingsViewModel>();

        services.AddTransient<SettingsView>();
        services.AddTransient<AuthorizationWindow>();
        services.AddSingleton<NewTaskView>();
        services.AddTransient<OverlayWindow>();
        services.AddTransient<MainWindow>();
    }
}

