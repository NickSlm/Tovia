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
            _appSettings = _settingsService.appSettings;

            ApplySettings();
        }

        private void ApplySettings()
        {
            var userSettings = _settingsService.userSettings;

            IsDarkTheme = userSettings.Appearance.BaseTheme == "dark" ? true : false;

            Theme theme = IsDarkTheme ?
                Theme.Create(BaseTheme.Dark,
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Secondary)) :
                Theme.Create(BaseTheme.Light,
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Secondary));
            _paletteHelper.SetTheme(theme);

        }

        public void ChangeTheme(bool newTheme)
        {
            Theme theme = newTheme ?
                Theme.Create(BaseTheme.Dark,
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Secondary)) :
                Theme.Create(BaseTheme.Light,
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Secondary));
            _paletteHelper.SetTheme(theme);
        }
    }
}
