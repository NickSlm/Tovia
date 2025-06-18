using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Options;
using System.Windows.Media;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class AppThemeService
    {
        public AppThemeService(IOptions<ThemeSettings> options)
        {

        }


        public void InitializeTheme()
        {

            bool IsLightMode = false;

            PaletteHelper palette = new PaletteHelper();
            Theme theme = IsLightMode ? 
                Theme.Create(BaseTheme.Light, Color.FromRgb(68,68,68), Color.FromRgb(20, 20, 20)) : 
                Theme.Create(BaseTheme.Dark, Color.FromRgb(255, 255, 0), Color.FromRgb(20, 20, 20));
            
            palette.SetTheme(theme);
        }
    }
}
