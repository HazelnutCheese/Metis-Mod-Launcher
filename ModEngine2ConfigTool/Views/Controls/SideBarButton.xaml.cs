using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace ModEngine2ConfigTool.Views.Controls
{
    public partial class SideBarButton : Button
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
                typeof(SideBarButton));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text), 
                typeof(string), 
                typeof(SideBarButton),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TextPropertyChanged));

        public static void TextPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        public SideBarButton()
        {
            InitializeComponent();
        }
    }
}
