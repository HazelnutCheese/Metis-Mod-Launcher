using ModEngine2ConfigTool.Extensions;
using ModEngine2ConfigTool.Views.Controls.Fields;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ModEngine2ConfigTool.Views.Converter
{
    public class AlignFieldsWidthViaParentScopeConverter : IValueConverter
    {
        private static readonly Dictionary<string, double> _widthScopes = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TextBlock textBlock = (TextBlock) value;
            var fieldsView = FrameworkElementExtensions.FindAncestorOfType<FieldsView>(textBlock);
            var fieldsViewOwner = fieldsView?.Parent;

            if(fieldsViewOwner is null)
            {
                return DependencyProperty.UnsetValue;
            }

            var scopeName = fieldsViewOwner.ToString();
            if(scopeName is null)
            {
                return DependencyProperty.UnsetValue;
            }

            var requestedWidth = CalculateRequestedWidth(textBlock);
            var scopeWidth = _widthScopes.GetValueOrDefault(scopeName);

            if (scopeWidth.Equals(default(double)) || requestedWidth > scopeWidth)
            {
                _widthScopes[scopeName] = requestedWidth;
            }

            return _widthScopes[scopeName];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static double CalculateRequestedWidth(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                textBlock.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    textBlock.FontFamily, 
                    textBlock.FontStyle, 
                    textBlock.FontWeight, 
                    textBlock.FontStretch),
                textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                VisualTreeHelper.GetDpi(textBlock).PixelsPerDip);

            return formattedText.Width + textBlock.Margin.Left + textBlock.Margin.Right;
        }
    }
}
