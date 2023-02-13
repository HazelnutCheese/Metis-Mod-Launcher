using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Helpers;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainPanelVm : ObservableObject
    {
        public NavigationService Navigator { get; }

        public MainPanelVm(
            NavigationService navigator, 
            ProfileManagerService profileManagerService, 
            ModManagerService modManagerService, 
            DllManagerService dllManagerService,
            PlayManagerService playManagerService,
            SaveManagerService saveManagerService)
        {
            Navigator = navigator;

            Navigator.CurrentPage = new HomePageVm(
                Navigator,
                profileManagerService,
                modManagerService,
                dllManagerService,
                playManagerService,
                saveManagerService);
        }
    }
}
