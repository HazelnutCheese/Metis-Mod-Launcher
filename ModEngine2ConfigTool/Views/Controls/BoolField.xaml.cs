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

namespace ModEngine2ConfigTool.Views.Controls
{
    public partial class BoolField : UserControl
    {
        public string FieldName
        {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }

        public static readonly DependencyProperty FieldNameProperty =
            DependencyProperty.Register(
                "FieldName", 
                typeof(string), 
                typeof(BoolField));

        public string FieldValue
        {
            get { return (string)GetValue(FieldValueProperty); }
            set { SetValue(FieldValueProperty, value); }
        }

        public static readonly DependencyProperty FieldValueProperty =
            DependencyProperty.Register(
                "FieldValue",
                typeof(bool),
                typeof(BoolField));

        public BoolField()
        {
            InitializeComponent();
        }
    }
}
