using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace organizer_application.Models
{
    public class ThemeViewModel : INotifyPropertyChanged
    {
        private ImageSource _currentIcon;
        private const string LightIconPath = "Images/Light_mode.png"; // Путь к PNG для светлой темы
        private const string DarkIconPath = "/Dark_mode.png";   // Путь к PNG для темной темы

        public ThemeViewModel()
        {
            // Инициализация темы по умолчанию через ThemeManager
            ThemeManager.CurrentThemeUri = ThemeManager.DarkThemeUri;
            CurrentIcon = LoadPng(DarkIconPath);

            // Инициализируем ToggleThemeCommand в конструкторе
            ToggleThemeCommand = new RelayCommand(ToggleThemeExecute);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource CurrentIcon
        {
            get { return _currentIcon; }
            set
            {
                _currentIcon = value;
                OnPropertyChanged(nameof(CurrentIcon));
            }
        }

        public ICommand ToggleThemeCommand { get; }

        private void ToggleThemeExecute()
        {
            ToggleTheme();
        }

        public void ToggleTheme()
        {
            if (ThemeManager.CurrentThemeUri == ThemeManager.LightThemeUri)
            {
                ThemeManager.CurrentThemeUri = ThemeManager.DarkThemeUri;
                CurrentIcon = LoadPng(DarkIconPath);
            }
            else
            {
                ThemeManager.CurrentThemeUri = ThemeManager.LightThemeUri;
                CurrentIcon = LoadPng(LightIconPath);
            }
        }

        private ImageSource LoadPng(string pngFileName)
        {
            try
            {
                // Загрузка PNG-изображения из ресурса
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = $"{assembly.GetName().Name}.{pngFileName.Replace('/', '.')}";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Console.WriteLine($"Resource not found: {resourceName}");
                        return null; // Или другое изображение по умолчанию
                    }
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze(); // Замораживаем изображение для потокобезопасности
                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading PNG: {ex.Message}");
                return null; // Или другое изображение по умолчанию
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
