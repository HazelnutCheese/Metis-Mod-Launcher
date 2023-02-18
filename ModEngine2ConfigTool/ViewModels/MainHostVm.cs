using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Services;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainHostVm : ObservableObject
    {
        public TopBarVm TopBarVm { get; }

        public MainPanelVm MainPanelVm { get; }

        public SideBarVm SideBarVm { get; }

        public MainHostVm(
            MainWindow mainWindow,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService,
            DllManagerService dllManagerService,
            PlayManagerService playManagerService)
        {
            TopBarVm = new TopBarVm(mainWindow, navigationService);

            MainPanelVm = new MainPanelVm(navigationService);

            SideBarVm = new SideBarVm(
                navigationService, 
                profileManagerService, 
                modManagerService,
                dllManagerService,
                playManagerService);
        }
    }
}
