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

            TopBarVm = new TopBarVm(mainWindow, navigationService);
            MainPanelVm = new MainPanelVm(navigationService);

            SideBarVm = new SideBarVm(
                navigationService, 
                profileManagerService, 
                modManagerService);
        }
    }
}
