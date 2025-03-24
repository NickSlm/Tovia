using System.Configuration;
using System.Data;
using System.Windows;
using ToDoListPlus.Services.oauth2;

namespace ToDoListPlus;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        // Initialize the authentication service here
        new AuthService();

        InitializeComponent();
    }
}

