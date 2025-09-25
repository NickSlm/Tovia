using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using System.Windows;
using System.Windows.Media;
using Tovia.Models;

namespace Tovia.Services
{
    public class AppThemeService
    {
        private PaletteHelper _paletteHelper = new PaletteHelper();
        private readonly SettingsService _settingsService;
        private readonly UserSettings _userSettings;
        private readonly AppSettings _appSettings;

        private static readonly Dictionary<string, Brush> DarkPalette = new()
        {
            {"MainWindowBorderBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33FFFFFF"))},
            {"OutlookHeaderBackgroundBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"))},
            {"OutlookHeaderForegroundBrush", Brushes.White },
            {"OutlookRowHoverBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"))},
            {"OutlookRowForegroundBrush", Brushes.WhiteSmoke},
            {"OutlookPanelBackgroundBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#121212"))},
            {"OutlookAccentBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005A9E"))},
            {"OutlookProgressTrackBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"))},
            {"OutlookButtonBackground", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B2B2B"))},
            {"OutlookButtonForeground", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0A84FF"))},
            {"OutlookButtonBorder", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#555555"))},
            {"OutlookButtonHover", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A3A3A"))},
            {"OutlookButtonEnabled", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A5F"))},
            {"OutlookButtonBackgroundDisabled", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F1F1F"))},
            {"OutlookButtonForegroundDisabled", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B6B6B"))},
            {"OutlookButtonBorderDisabled", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"))}
        };
        private static readonly Dictionary<string, Brush> LightPalette = new()
        {
            {"MainWindowBorderBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F000000"))},
            {"OutlookHeaderBackgroundBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4"))},
            {"OutlookHeaderForegroundBrush", Brushes.White },
            {"OutlookRowHoverBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F2F1"))},
            {"OutlookRowForegroundBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#201F1E"))},
            {"OutlookPanelBackgroundBrush", Brushes.White},
            {"OutlookAccentBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005A9E"))},
            {"OutlookProgressTrackBrush", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"))},
            {"OutlookButtonBackground", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"))},
            {"OutlookButtonForeground", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078D4"))},
            {"OutlookButtonBorder", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C7C7C7"))},
            {"OutlookButtonHover", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"))},
            {"OutlookButtonEnabled", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6F2FB"))},
            {"OutlookButtonBackgroundDisabled", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2F2F2"))},
            {"OutlookButtonForegroundDisabled", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A6A6A6"))},
            {"OutlookButtonBorderDisabled", new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D0D0D0"))}
        };

        public AppThemeService(SettingsService settingsService)
        {
            _settingsService = settingsService;
            _appSettings = _settingsService.appSettings;
            _userSettings = _settingsService.userSettings;

            var isDark = _userSettings.Appearance.BaseTheme == "dark";
            ChangeTheme(isDark);
        }

        public void ChangeTheme(bool newTheme)
        {
            SetTheme(newTheme);
            UpdatePalette(newTheme);
        }
        public void SetTheme(bool isDark)
        {
            Theme theme = isDark ?
                Theme.Create(BaseTheme.Dark,
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Dark.Secondary)) :
                Theme.Create(BaseTheme.Light,
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Primary),
                    (Color)ColorConverter.ConvertFromString(_appSettings.Palette.Light.Secondary));

            _paletteHelper.SetTheme(theme);
        }
        private void UpdatePalette(bool isDark)
        {
            var palette = isDark ? DarkPalette : LightPalette;

            foreach (var item in palette)
            {
                Application.Current.Resources[item.Key] = item.Value;
            };
        }
    }
}
