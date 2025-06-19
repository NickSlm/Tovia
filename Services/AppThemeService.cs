using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Options;
using System.Windows;
using System.Windows.Media;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class AppThemeService
    {

        public bool IsDarkMode;



        public AppThemeService(IOptions<ThemeSettings> options)
        {
            IsDarkMode = options.Value.BaseTheme == "light" ? false : true;
        }


        public void InitializeTheme()
        {
            // Initialize the theme based on the appsettings.json
            PaletteHelper palette = new PaletteHelper();
            Theme theme = IsDarkMode ?
                Theme.Create(BaseTheme.Dark, Color.FromRgb(255, 255, 0), Color.FromRgb(20, 20, 20)) :
                Theme.Create(BaseTheme.Light, Color.FromRgb(68, 68, 68), Color.FromRgb(20, 20, 20));
            palette.SetTheme(theme);
        }

        public void ChangeTheme()
        {

            //
        }
        public void UpdateTheme()
        {
            // Update the theme in appsettings.json for persistence

        }
    }
}
