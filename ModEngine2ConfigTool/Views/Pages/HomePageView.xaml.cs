using ModEngine2ConfigTool.Helpers;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for HomePageView.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {
        public HomePageView()
        {
            InitializeComponent();
        }

        private void ItemsControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(sender is ScrollViewer scrollViewer)
            {
                if (e.Delta < 0)
                {
                    scrollViewer.LineRight();
                    scrollViewer.LineRight();
                }
                else
                {
                    scrollViewer.LineLeft();
                    scrollViewer.LineLeft();
                }
                e.Handled = true;
            }
        }
    }
}
