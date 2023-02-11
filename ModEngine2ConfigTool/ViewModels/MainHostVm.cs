using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Services;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainHostVm : ObservableObject
    {
        public TopBarVm TopBarVm { get; }

        public MainPanelVm MainPanelVm { get; }

        public SideBarVm SideBarVm { get; }

        public MainHostVm(MainWindow mainWindow)
        {
            var navigationService = new NavigationService();

            var profileManagerService = new ProfileManagerService(
                App.DatabaseService,
                App.DispatcherService);

            var modManagerService = new ModManagerService(
                App.DatabaseService,
                App.DispatcherService,
                profileManagerService);

            var dllManagerService = new DllManagerService(
                App.DatabaseService,
                App.DispatcherService,
                profileManagerService);

            var playManagerService = new PlayManagerService(
                App.ProfileService,
                App.SaveManagerService,
                App.ModEngine2Service,
                App.DispatcherService);

            TopBarVm = new TopBarVm(mainWindow, navigationService);

            MainPanelVm = new MainPanelVm(
                navigationService,
                profileManagerService,
                modManagerService,
                dllManagerService,
                playManagerService,
                App.SaveManagerService);

            SideBarVm = new SideBarVm(
                navigationService, 
                profileManagerService, 
                modManagerService,
                dllManagerService,
                playManagerService,
                App.SaveManagerService);
        }
    }
}
