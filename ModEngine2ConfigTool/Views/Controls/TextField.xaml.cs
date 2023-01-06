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
    /// <summary>
    /// Interaction logic for TextField.xaml
    /// </summary>
    /// 
    public partial class TextField : UserControl
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
                typeof(TextField));

        public string FieldValue
        {
            get { return (string)GetValue(FieldValueProperty); }
            set { SetValue(FieldValueProperty, value); }
        }

        public static readonly DependencyProperty FieldValueProperty =
            DependencyProperty.Register(
                "FieldValue",
                typeof(string),
                typeof(TextField));

        public string FieldActionName
        {
            get { return (string)GetValue(FieldActionNameProperty); }
            set { SetValue(FieldActionNameProperty, value); }
        }

        public static readonly DependencyProperty FieldActionNameProperty =
            DependencyProperty.Register(
                "FieldActionName", 
                typeof(string), 
                typeof(TextField));

        public ICommand FieldAction
        {
            get { return (ICommand)GetValue(FieldActionProperty); }
            set { SetValue(FieldActionProperty, value); }
        }

        public static readonly DependencyProperty FieldActionProperty =
            DependencyProperty.Register(
                "FieldAction", 
                typeof(ICommand), 
                typeof(TextField));

        public TextField()
        {
            InitializeComponent();
        }
    }
}
