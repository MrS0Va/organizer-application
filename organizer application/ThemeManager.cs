using System;
using System.Windows;

namespace organizer_application
{
    public static class ThemeManager
    {
        // Событие для уведомления об изменении темы
        public static event EventHandler ThemeChanged;

        private static Uri _currentThemeUri;

        public static Uri LightThemeUri { get; } = new Uri("Theme/LightTheme.xaml", UriKind.Relative);
        public static Uri DarkThemeUri { get; } = new Uri("Theme/DarkTheme.xaml", UriKind.Relative);


        public static Uri CurrentThemeUri
        {
            get { return _currentThemeUri; }
            set
            {
                if (_currentThemeUri != value)
                {
                    _currentThemeUri = value;
                    ApplyTheme(_currentThemeUri);
                    ThemeChanged?.Invoke(null, EventArgs.Empty); // Уведомляем об изменении
                }
            }
        }

        private static void ApplyTheme(Uri themeUri)
        {
            try
            {
                var themeDictionary = new ResourceDictionary { Source = themeUri };

                // Найдите и удалите старый словарь темы
                ResourceDictionary oldTheme = null;
                foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
                {
                    if (dictionary.Source != null && (dictionary.Source.Equals(LightThemeUri) || dictionary.Source.Equals(DarkThemeUri)))
                    {
                        oldTheme = dictionary;
                        break;
                    }
                }

                if (oldTheme != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(oldTheme);
                }


                // Добавьте новый словарь темы
                Application.Current.Resources.MergedDictionaries.Add(themeDictionary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при применении темы: {ex.Message}");
                // Обработайте ошибку, возможно, установите тему по умолчанию
            }
        }
    }
}
