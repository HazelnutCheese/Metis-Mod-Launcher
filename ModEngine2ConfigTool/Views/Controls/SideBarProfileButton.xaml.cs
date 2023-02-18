using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace ModEngine2ConfigTool.Views.Controls
{
    public partial class SideBarProfileButton : Button
    {
        public PackIconKind IconKind
        {
            get { return (PackIconKind)GetValue(IconKindProperty); }
            set { SetValue(IconKindProperty, value); }
        }

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(
                nameof(IconKind), 
                typeof(PackIconKind), 
                typeof(SideBarProfileButton));

        public SideBarProfileButton()
        {
            InitializeComponent();
        }
    }
}
