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
            IDatabaseService databaseService,
            IDispatcherService dispatcherService,
            ProfileService profileService,
            SaveManagerService saveManagerService,
            ModEngine2Service modEngine2Service)
        {
            var navigationService = new NavigationService();

            var profileManagerService = new ProfileManagerService(
                databaseService,
                dispatcherService);

            var modManagerService = new ModManagerService(
                databaseService,
                dispatcherService,
                profileManagerService);

            var dllManagerService = new DllManagerService(
                databaseService,
                dispatcherService,
                profileManagerService);

            var playManagerService = new PlayManagerService(
                profileService,
                saveManagerService,
                modEngine2Service,
                dispatcherService);

            TopBarVm = new TopBarVm(mainWindow, navigationService);

            MainPanelVm = new MainPanelVm(
                navigationService,
                profileManagerService,
                modManagerService,
                dllManagerService,
                playManagerService,
                saveManagerService);

            SideBarVm = new SideBarVm(
                navigationService, 
                profileManagerService, 
                modManagerService,
                dllManagerService,
                playManagerService,
                saveManagerService);
        }
    }
}
