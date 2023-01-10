using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ModEngine2ConfigTool.Views.Converter
{
    public class EmptyImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string imagePath && !string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                return imagePath;
            }

            return Path.GetFullPath(".\\Resources\\EldenRing256.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
