using System.Windows;
using ToDoListPlus.ViewModels;

namespace ToDoListPlus.Views
{
    /// <summary>
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow(AuthorizationViewModel authorizationVM)
        {
            DataContext = authorizationVM;
            InitializeComponent();
        }
    }
}
