using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Options;
using System.Windows;
using System.Windows.Media;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class AppThemeService
    {

        public bool IsDarkTheme;
        private PaletteHelper _paletteHelper = new PaletteHelper();
        private readonly SettingsService _settingsService;


        public AppThemeService(SettingsService settingsService)
        {
            _settingsService = settingsService;

            var conf = _settingsService.Load();

            var settings = conf.Get<UserSettings>();
            IsDarkTheme = settings.Theme.BaseTheme == "light" ? false : true;
        }
        public void InitializeTheme()
        {
            // Initialize the theme based on the appsettings.json
            Theme theme = IsDarkTheme ?
                Theme.Create(BaseTheme.Dark, Color.FromRgb(255, 255, 0), Color.FromRgb(20, 20, 20)):
                Theme.Create(BaseTheme.Light, Color.FromRgb(68,68,68), Color.FromRgb(20, 20, 20));
            _paletteHelper.SetTheme(theme);
        }

        public void ChangeTheme(bool newTheme)
        {
            Theme theme = newTheme ?
               Theme.Create(BaseTheme.Dark, Color.FromRgb(255, 255, 0), Color.FromRgb(20, 20, 20)) :
               Theme.Create(BaseTheme.Light, Color.FromRgb(68, 68, 68), Color.FromRgb(20, 20, 20));
            _paletteHelper.SetTheme(theme);
        }
    }
}
