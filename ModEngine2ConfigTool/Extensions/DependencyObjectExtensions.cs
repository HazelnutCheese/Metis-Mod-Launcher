using System.Windows;
using System.Windows.Media;

namespace ModEngine2ConfigTool.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static T? FindAncestorOfType<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            if (LogicalTreeHelper.GetParent(dependencyObject) is not DependencyObject parent)
            {
                return null;
            }

            if (parent is T parentOfType)
            {
                return parentOfType;
            }

            return FindAncestorOfType<T>(parent);
        }
    }
}
