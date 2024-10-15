using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MunicipalServicesApplication.Services
{
    public class SvgToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imageUrl)
            {
                Console.WriteLine($"Converting image URL: {imageUrl}");

                if (imageUrl.StartsWith("data:image/svg+xml,"))
                {
                    Console.WriteLine("SVG detected, using placeholder");
                    return new BitmapImage(new Uri("pack://application:,,,/Resources/placeholderImage.png"));
                }
                else if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uri))
                {
                    try
                    {
                        return new BitmapImage(uri);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading image: {ex.Message}");
                        return new BitmapImage(new Uri("pack://application:,,,/Resources/placeholderImage.png"));
                    }
                }
            }

            Console.WriteLine("Invalid or null image URL, using placeholder");
            return new BitmapImage(new Uri("pack://application:,,,/Resources/placeholderImage.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
