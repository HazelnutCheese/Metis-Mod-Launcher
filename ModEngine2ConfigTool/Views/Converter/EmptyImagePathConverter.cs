using System;
using System.Collections.Generic;
using System.Drawing;
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
                return CreateBitmap(imagePath);
            }

            if(parameter is not string fallbackPath)
            {
                return DependencyProperty.UnsetValue;
            }

            return CreateBitmap($".\\Resources\\{fallbackPath}.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapFrame CreateBitmap(string path)
        {
            try
            {
                var encoder = new PngBitmapEncoder();
                var image = new BitmapImage(new Uri(path, UriKind.Relative))
                {
                    CreateOptions = BitmapCreateOptions.IgnoreImageCache,
                    CacheOption = BitmapCacheOption.OnLoad
                };

                encoder.Frames.Add(BitmapFrame.Create(image));

                return encoder.Frames[0];
            }
            catch
            {
                return BitmapFrame.Create(CreateEmptyBitmap());
            }
        }

        private BitmapSource CreateEmptyBitmap()
        {
            int width = 128;
            int height = width;
            int stride = width / 8;
            byte[] pixels = new byte[height * stride];

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Black);
            BitmapPalette myPalette = new BitmapPalette(colors);

            // Creates a new empty image with the pre-defined palette
            return BitmapSource.Create(
                width, height,
                96, 96,
                PixelFormats.Indexed1,
                myPalette,
                pixels,
                stride);
        }
    }
}
