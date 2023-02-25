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
    /// Interaction logic for ModsPageView.xaml
    /// </summary>
    public partial class ModsPageView : UserControl
    {
        public ModsPageView()
        {
            InitializeComponent();
        }

        private void ProfileScroller_MouseWheel(object sender, ScrollChangedEventArgs e)
        {

            const double offset = 275;

            if (e.VerticalOffset > offset)
            {
                GridHeader.RenderTransform = new TranslateTransform(0, e.VerticalOffset - offset);
            }
            else
            {
                GridHeader.RenderTransform = new TranslateTransform(0, 0);
            }
        }
    }
}
