using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Windows.Input;
using Tovia.Models;
using Tovia.Services;

namespace Tovia.Views
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {

        private readonly SettingsService _settingsService;

        public OverlayWindow(SettingsService settingsService)
        {
            _settingsService = settingsService;
            InitializeComponent();
        }

        public void Window_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();

                SaveWindowCords();
            }
        }

        public void SaveWindowCords()
        {
            

            MessageBox.Show($"{this.Top}");
        }
    }
}
