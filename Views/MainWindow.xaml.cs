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


        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;
    }


    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.ToDoListVM.LoadToDoItems();
    }
    private void MainWindow_Closed(object sender, EventArgs e)
    {
        Application.Current.Shutdown();
    }



}