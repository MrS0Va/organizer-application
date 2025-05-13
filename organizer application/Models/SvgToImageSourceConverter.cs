using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using SkiaSharp;
using Svg.Skia;

namespace organizer_application.Models
{
    public class SvgToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string svgFileName)
            {
                try
                {
                    // Загрузка SVG-изображения из ресурса
                    var assembly = Assembly.GetExecutingAssembly();
                    string resourceName = $"{assembly.GetName().Name}.{svgFileName}";

                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null)
                        {
                            return null; // Или другое изображение по умолчанию
                        }

                        var svg = new SKSvg();
                        svg.Load(stream);

                        var bitmap = svg.Picture.ToBitmap(SkiaSharp.SKColors.Transparent, 1.0f, 1.0f, SkiaSharp.SKColorType.Rgba8888, SkiaSharp.SKAlphaType.Premul, SkiaSharp.SKColorSpace.CreateSrgb());
                        //return bitmap.AsStream().ToBitmapImage();
                        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                               bitmap.Handle,
                               IntPtr.Zero,
                               System.Windows.Int32Rect.Empty,
                               System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибок (запись в лог, отображение изображения по умолчанию)
                    Console.WriteLine($"Error loading SVG: {ex.Message}");
                    return null; // Или другое изображение по умолчанию
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
