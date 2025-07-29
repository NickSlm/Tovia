using System.Windows;
using ToDoListPlus.ViewModels;

namespace ToDoListPlus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
/// 
    

public partial class MainWindow
{

    private readonly MainViewModel _viewModel;
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        Closed += MainWindow_Closed;
    }

    private void MainWindow_Closed(object sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }

}