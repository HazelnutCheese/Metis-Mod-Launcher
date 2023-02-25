using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModEngine2ConfigTool.Views.Converter
{
    [ValueConversion(typeof(String), typeof(ImageSource))]
    public class EmptyImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string imagePath && !string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                return BitmapFrame.Create(new Uri(imagePath), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
            }

            if(parameter is not string fallbackPath)
            {
                return DependencyProperty.UnsetValue;
            }

            return BitmapFrame.Create(new Uri(Path.GetFullPath($".\\Resources\\{fallbackPath}.png")), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
