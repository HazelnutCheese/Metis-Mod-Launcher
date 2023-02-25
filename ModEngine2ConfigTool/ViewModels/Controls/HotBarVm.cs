using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class HotBarVm : ObservableObject
    {
        public ObservableCollection<ObservableObject> Buttons { get; }

        public HotBarVm(ObservableCollection<ObservableObject> buttons)
        {
            Buttons = buttons;
        }
    }
}
