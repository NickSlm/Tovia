using System.Configuration;
using System.Data;
using System.Windows;
using ToDoListPlus.Services;
using Microsoft.Extensions.DependencyInjection;
using ToDoListPlus.ViewModels;
using Microsoft.EntityFrameworkCore;

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

        var dbInitializer = _serviceProvider.GetRequiredService<IDatabaseInitializer>();
        dbInitializer.Initialize();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {

        var dbContext = _serviceProvider.GetService<ToDoContext>();
        if (dbContext != null)
        {
            await dbContext.SaveChangesAsync();
        }
        base.OnExit(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ToDoContext>(options => options.UseSqlite("Data Source=ToDoList.db"));

        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
        services.AddSingleton<AuthService>();

        services.AddSingleton<ToDoListViewModel>();
        services.AddSingleton<AuthorizationViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddSingleton<MainWindow>();
    }
}

