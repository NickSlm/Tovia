using System.Windows;
using ToDoListPlus.ViewModels;

namespace ToDoListPlus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
/// 
    

public partial class MainWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
    private void MainWindow_Closed(object sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }
}