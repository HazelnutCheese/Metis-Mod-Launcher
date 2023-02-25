using System.ComponentModel;
using System.Windows;

namespace ModEngine2ConfigTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MaxHeight = SystemParameters.VirtualScreenHeight - 32;
            MaxWidth = SystemParameters.VirtualScreenWidth + 10;
        }
    }
}
