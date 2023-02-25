using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainPanelVm : ObservableObject
    {
        public NavigationService Navigator { get; }

        public MainPanelVm(
            NavigationService navigator)
        {
            Navigator = navigator;
        }
    }
}
