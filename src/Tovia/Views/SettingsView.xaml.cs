using System.Windows.Controls;
using System.Windows.Input;
using Tovia.ViewModels;

namespace Tovia.Views
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
            if (viewModel is SettingsViewModel settingsViewModel && sender is TextBox textBox)
            {
                string name = textBox.Tag as string;

                switch (name)
                {
                    case "Overlay":
                        settingsViewModel._hotkeySettings[name] = (key, modifiers);
                        settingsViewModel._keyStrokes[name].keyStroke = $"{modifiers.ToString()} + {key.ToString()}";
                        break;
                    case "NewTask":
                        settingsViewModel._hotkeySettings[name] = (key, modifiers);
                        settingsViewModel._keyStrokes[name].keyStroke = $"{modifiers.ToString()} + {key.ToString()}";
                        break;
                }
            }
        }
    }
}
