using System.Configuration;
using System.Data;
using System.Windows;
using ToDoListPlus.Services;
using Microsoft.Extensions.DependencyInjection;
using ToDoListPlus.ViewModels;
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

        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<AuthService>();

        services.AddSingleton<ToDoListViewModel>();
        services.AddSingleton<AuthorizationViewModel>();
        services.AddSingleton<PopupDialogViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddSingleton<PopupDialogView>();
        services.AddSingleton<MainWindow>();

    }
}

