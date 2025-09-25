using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Tovia.ViewModels;

namespace Tovia;

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
    private void Close_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState =
            this.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }
}