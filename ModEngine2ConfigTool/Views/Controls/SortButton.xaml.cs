using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ModEngine2ConfigTool.Views.Controls
{
    public partial class SortButton : Button
    {
        private static readonly Dictionary<string, List<SortButton>> _sortGroups;

        public SortButtonMode Mode
        {
            get { return (SortButtonMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                nameof(Mode),
                typeof(SortButtonMode),
                typeof(SortButton),
                new FrameworkPropertyMetadata(SortButtonMode.Off));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(SortButton));

        public string SortGroup
        {
            get { return (string)GetValue(SortGroupProperty); }
            set { SetValue(SortGroupProperty, value); }
        }

        public static readonly DependencyProperty SortGroupProperty =
            DependencyProperty.Register(
                nameof(SortGroup),
                typeof(string),
                typeof(SortButton),
                new FrameworkPropertyMetadata(string.Empty, SortGroupChanged));

        public SortButton()
        {
            InitializeComponent();
        }

        static SortButton()
        {
            _sortGroups = new Dictionary<string, List<SortButton>>();
        }

        private static void SortGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is not SortButton sortButton)
            {
                return;
            }

            if(e.OldValue is string oldGroup 
                && !string.IsNullOrWhiteSpace(oldGroup) 
                && _sortGroups.TryGetValue(oldGroup, out var oldGroupList) 
                && oldGroupList.Contains(sortButton))
            {
                oldGroupList.Remove(sortButton);
                if (oldGroupList.Count == 0)
                {
                    _sortGroups.Remove(oldGroup);
                }
            }

            if(e.NewValue is string newGroup && !string.IsNullOrWhiteSpace(newGroup))
            {
                if(_sortGroups.TryGetValue(newGroup, out var newGroupList))
                {
                    if(newGroupList.Contains(sortButton))
                    {
                        return;
                    }

                    newGroupList.Add(sortButton);
                }
                else
                {
                    var list = new List<SortButton>
                    {
                        sortButton
                    };

                    _sortGroups.Add(newGroup, list);
                }
            }
        }

        private void SortButtonSelf_Click(object sender, RoutedEventArgs e)
        {
            if(sender is not SortButton sortButton)
            {
                return;
            }

            if(sortButton.GetValue(SortGroupProperty) is not string sortGroup || string.IsNullOrWhiteSpace(sortGroup))
            {
                return;
            }

            if(!_sortGroups.TryGetValue(sortGroup, out var sortGroupList))
            {
                return;
            }

            foreach(var otherButton in sortGroupList.Where(x => x != sortButton))
            {
                otherButton.Mode = SortButtonMode.Off;
            }

            if(sortButton.Mode == SortButtonMode.Off || sortButton.Mode == SortButtonMode.Ascending) 
            {
                sortButton.Mode = SortButtonMode.Descending;
            }
            else
            {
                sortButton.Mode = SortButtonMode.Ascending;
            }
        }
    }
}
