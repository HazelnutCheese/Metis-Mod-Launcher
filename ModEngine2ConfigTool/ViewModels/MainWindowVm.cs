using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Services;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainWindowVm : ObservableObject
    {
        public MainHostVm MainHostVm { get; }

        public MainWindowVm(
            MainWindow mainWindow,
            IDatabaseService databaseService,
            IDispatcherService dispatcherService,
            ProfileService profileService,
            SaveManagerService saveManagerService,
            ModEngine2Service modEngine2Service)
        {
            MainHostVm = new MainHostVm(
                mainWindow,
                databaseService,
                dispatcherService,
                profileService,
                saveManagerService,
                modEngine2Service);
        }
    }
}
