using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.ViewModels;
using ModEngine2ConfigTool.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ModEngine2ConfigTool.Views.Controls
{
    /// <summary>
    /// Interaction logic for ModOrderView.xaml
    /// </summary>
    public partial class ModOrderView : UserControl
    {
        public ModOrderView()
        {
            InitializeComponent();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            var expander = (Expander)sender;
            var row = Grid.GetRow(expander);
            ((Grid)((ModOrderView)expander.Parent).Parent).RowDefinitions[row].Height = new GridLength(1, GridUnitType.Star);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            var expander = (Expander)sender;
            var row = Grid.GetRow(expander);
            ((Grid)((ModOrderView)expander.Parent).Parent).RowDefinitions[row].Height = new GridLength(1, GridUnitType.Auto);
        }
    }
}
