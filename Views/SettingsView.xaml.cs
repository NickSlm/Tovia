using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using ToDoListPlus.Services;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using ToDoListPlus.ViewModels;
using ToDoListPlus.Models;

namespace ToDoListPlus.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {

        public SettingsView()
        {
            InitializeComponent();
        }

        private void KeyBind_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            ModifierKeys modifiers = Keyboard.Modifiers;

            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            var viewModel = this.DataContext;
            if (viewModel is SettingsViewModel settingsViewModel)
            {
                settingsViewModel.MainKey = key;
                settingsViewModel.ModifierKey = modifiers;
                settingsViewModel.KeyStroke = $"{modifiers.ToString()} + {key.ToString()}";
            }
        }
    }
}
