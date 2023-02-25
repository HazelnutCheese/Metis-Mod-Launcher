using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ModEngine2ConfigTool.Views.Converter
{
    public class SelectedItemColourConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var selectedObject = values[0];
            var currentObject = values[1];

            if(selectedObject.Equals(currentObject))
            {
                return new SolidColorBrush((Color) ColorConverter.ConvertFromString("#ffc969"));
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d1d1d1"));
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
