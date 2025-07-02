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
            var userSettings = conf.Get<UserSettings>();

            IsDarkTheme = userSettings.Theme.BaseTheme == "dark" ? true : false;
        }
        public void InitializeTheme()
        {


            var conf = _settingsService.Load();
            var appSettings = conf.Get<AppSettings>();

            Theme theme = IsDarkTheme ?
                Theme.Create(BaseTheme.Dark,
                    (Color)ColorConverter.ConvertFromString(appSettings.Palette.Dark.Primary),
                    (Color)ColorConverter.ConvertFromString(appSettings.Palette.Dark.Primary)) :

                Theme.Create(BaseTheme.Light,
                    (Color)ColorConverter.ConvertFromString(appSettings.Palette.Light.Primary),
                    (Color)ColorConverter.ConvertFromString(appSettings.Palette.Light.Primary));
            _paletteHelper.SetTheme(theme);
        }

        public void ChangeTheme(bool isDarkTheme)
        {

            var conf = _settingsService.Load();
            var appSettings = conf.Get<AppSettings>();

            Theme theme = isDarkTheme ?
                Theme.Create(BaseTheme.Dark, 
                    (Color)ColorConverter.ConvertFromString(appSettings.Palette.Dark.Primary),
                    (Color)ColorConverter.ConvertFromString(appSettings.Palette.Dark.Primary)):

                Theme.Create(BaseTheme.Light,
                    (Color)ColorConverter.ConvertFromString(appSettings.Palette.Light.Primary),
                    (Color)ColorConverter.ConvertFromString(appSettings.Palette.Light.Primary));
            _paletteHelper.SetTheme(theme);
        }
    }
}
