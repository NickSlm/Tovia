using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToDoListPlus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
/// 


public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        var todolist = new ToDoListViewModel();
        DataContext = todolist;
    }
}