using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;

using System.Windows.Media;
using ToDoListPlus.Models;

namespace ToDoListPlus.Services
{
    public class AppThemeService
    {

        public bool IsDarkTheme;
        private PaletteHelper _paletteHelper = new PaletteHelper();
        private readonly SettingsService _settingsService;
        private readonly AppSettings _appSettings;

        public AppThemeService(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _settingsService.Load();

            var userSettings = _settingsService.userSettings;
            _appSettings = _settingsService.appSettings;

            IsDarkTheme = userSettings.Appearance.BaseTheme == "dark" ? true : false;
        }

        public void InitializeTheme()
        {
            Theme theme = IsDarkTheme ?
                Theme.Create(BaseTheme.Dark,
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Secondary)):
                Theme.Create(BaseTheme.Light,
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Secondary));
            _paletteHelper.SetTheme(theme);
        }

        public void ChangeTheme(bool isDarkTheme)
        {
            Theme theme = isDarkTheme ?
                Theme.Create(BaseTheme.Dark, 
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Secondary)):
                Theme.Create(BaseTheme.Light,
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Secondary));
            _paletteHelper.SetTheme(theme);
        }
    }
}
