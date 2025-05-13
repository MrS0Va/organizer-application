using Svg.Skia;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Interop;
using SkiaSharp;

namespace organizer_application.Models
{
    public class ThemeViewModel : INotifyPropertyChanged
    {
        private ImageSource _currentIcon;
        private const string LightIconPath = "Images.light_icon.svg";
        private const string DarkIconPath = "Images.dark_icon.svg";

        public ThemeViewModel()
        {
            // Инициализация темы по умолчанию через ThemeManager
            ThemeManager.CurrentThemeUri = ThemeManager.LightThemeUri;
            CurrentIcon = LoadSvg(LightIconPath);

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

        // Убираем инициализацию из объявления поля
        public ICommand ToggleThemeCommand { get; } // Make sure to remove private set

        private void ToggleThemeExecute()
        {
            ToggleTheme();
        }

        public void ToggleTheme()
        {
            if (ThemeManager.CurrentThemeUri == ThemeManager.LightThemeUri)
            {
                ThemeManager.CurrentThemeUri = ThemeManager.DarkThemeUri;
                CurrentIcon = LoadSvg(DarkIconPath);
            }
            else
            {
                ThemeManager.CurrentThemeUri = ThemeManager.LightThemeUri;
                CurrentIcon = LoadSvg(LightIconPath);
            }
        }

        private ImageSource LoadSvg(string svgFileName)
        {
            // Загрузка SVG-изображения из ресурса
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"{assembly.GetName().Name}.{svgFileName}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null; // Или другое изображение по умолчанию
                }

                SKSvg svg = new Svg.Skia.SKSvg();
                svg.Load(stream);

                SKBitmap bitmap = svg.Picture.ToBitmap(SKColors.Transparent, 1.0f, 1.0f, SKColorType.Rgba8888, SKAlphaType.Premul, SKColorSpace.CreateSrgb());

                return Imaging.CreateBitmapSourceFromHBitmap(
                       bitmap.Handle,
                       IntPtr.Zero,
                       Int32Rect.Empty,
                       System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
