using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Windows.Media;
using Tovia.Models;

namespace Tovia.Services
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

            if (IsDarkTheme)
            {
                Application.Current.Resources["OutlookHeaderBackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                Application.Current.Resources["OutlookHeaderForegroundBrush"] = Brushes.White;
                Application.Current.Resources["OutlookRowHoverBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
                Application.Current.Resources["OutlookRowForegroundBrush"] = Brushes.WhiteSmoke;
                Application.Current.Resources["OutlookPanelBackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#121212"));
                Application.Current.Resources["OutlookAccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005A9E"));
                Application.Current.Resources["OutlookProgressTrackBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
                Application.Current.Resources["OutlookButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B2B2B"));
                Application.Current.Resources["OutlookButtonForeground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0A84FF"));
                Application.Current.Resources["OutlookButtonBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555555"));
                Application.Current.Resources["OutlookButtonHover"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3A3A"));
                Application.Current.Resources["OutlookButtonEnabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F"));
                Application.Current.Resources["OutlookButtonBackgroundDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F1F1F"));
                Application.Current.Resources["OutlookButtonForegroundDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B6B6B"));
                Application.Current.Resources["OutlookButtonBorderDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"));
            }
            else
            {
                Application.Current.Resources["OutlookHeaderBackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4"));
                Application.Current.Resources["OutlookHeaderForegroundBrush"] = Brushes.White;
                Application.Current.Resources["OutlookRowHoverBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F2F1"));
                Application.Current.Resources["OutlookRowForegroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#201F1E"));
                Application.Current.Resources["OutlookPanelBackgroundBrush"] = Brushes.White;
                Application.Current.Resources["OutlookAccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005A9E"));
                Application.Current.Resources["OutlookProgressTrackBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
                Application.Current.Resources["OutlookButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                Application.Current.Resources["OutlookButtonForeground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4"));
                Application.Current.Resources["OutlookButtonBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C7C7C7"));
                Application.Current.Resources["OutlookButtonHover"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                Application.Current.Resources["OutlookButtonEnabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6F2FB"));
                Application.Current.Resources["OutlookButtonBackgroundDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2F2F2"));
                Application.Current.Resources["OutlookButtonForegroundDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A6A6A6"));
                Application.Current.Resources["OutlookButtonBorderDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D0D0D0"));
            }
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

            if (newTheme)
            {
                Application.Current.Resources["OutlookHeaderBackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                Application.Current.Resources["OutlookHeaderForegroundBrush"] = Brushes.White;
                Application.Current.Resources["OutlookRowHoverBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
                Application.Current.Resources["OutlookRowForegroundBrush"] = Brushes.WhiteSmoke;
                Application.Current.Resources["OutlookPanelBackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#121212"));
                Application.Current.Resources["OutlookAccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005A9E"));
                Application.Current.Resources["OutlookProgressTrackBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
                Application.Current.Resources["OutlookButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B2B2B"));
                Application.Current.Resources["OutlookButtonForeground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0A84FF"));
                Application.Current.Resources["OutlookButtonBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555555"));
                Application.Current.Resources["OutlookButtonHover"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3A3A"));
                Application.Current.Resources["OutlookButtonEnabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F"));
                Application.Current.Resources["OutlookButtonBackgroundDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F1F1F"));
                Application.Current.Resources["OutlookButtonForegroundDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B6B6B"));
                Application.Current.Resources["OutlookButtonBorderDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"));
            }
            else
            {
                Application.Current.Resources["OutlookHeaderBackgroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4"));
                Application.Current.Resources["OutlookHeaderForegroundBrush"] = Brushes.White;
                Application.Current.Resources["OutlookRowHoverBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F2F1"));
                Application.Current.Resources["OutlookRowForegroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#201F1E"));
                Application.Current.Resources["OutlookPanelBackgroundBrush"] = Brushes.White;
                Application.Current.Resources["OutlookAccentBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005A9E"));
                Application.Current.Resources["OutlookProgressTrackBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
                Application.Current.Resources["OutlookButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                Application.Current.Resources["OutlookButtonForeground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4"));
                Application.Current.Resources["OutlookButtonBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C7C7C7"));
                Application.Current.Resources["OutlookButtonHover"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                Application.Current.Resources["OutlookButtonEnabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6F2FB"));
                Application.Current.Resources["OutlookButtonBackgroundDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2F2F2"));
                Application.Current.Resources["OutlookButtonForegroundDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A6A6A6"));
                Application.Current.Resources["OutlookButtonBorderDisabled"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D0D0D0"));
            }
        }
    }
}
