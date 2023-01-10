using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModEngine2ConfigTool.Views.Pages
{
    /// <summary>
    /// Interaction logic for ProfilesPageView.xaml
    /// </summary>
    public partial class ProfilesPageView : UserControl
    {
        public ProfilesPageView()
        {
            InitializeComponent();
        }

        public T FindElementByName<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
        {
            T childElement = null;
            var nChildCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < nChildCount; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(sChildName))
                {
                    childElement = (T)child;
                    break;
                }

                childElement = FindElementByName<T>(child, sChildName);

                if (childElement != null)
                    break;
            }
            return childElement;
        }

        private void ProfileScroller_MouseWheel(object sender, ScrollChangedEventArgs e)
        {
            var gridHeader = FindElementByName<Grid>(ProfileContentView, "GridHeader");
            if (gridHeader is null)
            {
                return;
            }

            if(e.VerticalOffset > 137)
            {
                gridHeader.RenderTransform = new TranslateTransform(0, e.VerticalOffset - 137);
            }
            else
            {
                gridHeader.RenderTransform = new TranslateTransform(0, 0);
            }
        }
    }
}
