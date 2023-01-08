using ModEngine2ConfigTool.ViewModels.Fields;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ModEngine2ConfigTool.Views.Controls.Fields
{
    public partial class FieldsView : UserControl
    {
        public ObservableCollection<IFieldViewModel> Fields
        {
            get { return (ObservableCollection<IFieldViewModel>)GetValue(FieldsProperty); }
            set { SetValue(FieldsProperty, value); }
        }

        public static readonly DependencyProperty FieldsProperty =
            DependencyProperty.Register(
                nameof(Fields), 
                typeof(ObservableCollection<IFieldViewModel>), 
                typeof(FieldsView));

        public FieldsView()
        {
            InitializeComponent();
        }
    }
}
