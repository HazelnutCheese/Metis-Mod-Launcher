using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Services;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainWindowVm : ObservableObject
    {
        public MainHostVm MainHostVm { get; }

        public MainWindowVm(
            MainWindow mainWindow,
            IContainer serviceContainer)
        {
            var navigationService = serviceContainer.Resolve<NavigationService>();
            navigationService.Initialise(serviceContainer);

            MainHostVm = new MainHostVm(
                mainWindow,
                serviceContainer.Resolve<NavigationService>(),
                serviceContainer.Resolve<ProfileManagerService>(),
                serviceContainer.Resolve<ModManagerService>(),
                serviceContainer.Resolve<DllManagerService>(),
                serviceContainer.Resolve<PlayManagerService>());
        }
    }
}
