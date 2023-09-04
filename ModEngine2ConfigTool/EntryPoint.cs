using Autofac;
using ModEngine2ConfigTool.Helpers;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using PowerArgs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace ModEngine2ConfigTool
{
    public class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using var otherProcess = GetOtherInstance();
            if (otherProcess != null)
            {
                MessageBox.Show("Only one instance of Metis Mod Launcher can be running.");
                WindowHelper.BringProcessToFront(otherProcess);
                otherProcess.Dispose();
                return;
            }

            var commandLineArgs = Args.Parse<CommandLineArgs>(args);

            var appDataPath = commandLineArgs.AppData
                ?? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Metis Mod Launcher");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            var configPath = Path.Combine(appDataPath, "appsettings.json");

            var serviceContainer = GetServicesContainer(appDataPath, configPath, !(commandLineArgs.ProfileId is null));

            if (commandLineArgs.ProfileId is null)
            {
                
                Start(serviceContainer);
            }
            else
            {
                var dispatcherService = new NoUiDispatcher();
                StartSilent(
                    commandLineArgs.ProfileId,
                    serviceContainer);
            }
        }

        private static void Start(IContainer serviceContainer)
        {
            var app = new App(serviceContainer);
            app.InitializeComponent();
            app.Run();
        }

        private static void StartSilent(
            string profileId,
            IContainer serviceContainer)
        {
            var playManagerService = serviceContainer.Resolve<PlayManagerService>();
            var profileManager = serviceContainer.Resolve<ProfileManagerService>();

            var profile = profileManager
                .ProfileVms
                .FirstOrDefault(x => x.Model.ProfileId.ToString() == profileId);

            if (profile is null)
            {
                MessageBox.Show($"Could not find profile: \"{profileId}\"");
                return;
            }

            playManagerService.PlaySilent(profile);
        }

        private static IContainer GetServicesContainer(
            string appDataPath,
            string configPath,
            bool isSilentMode)
        {
            var serviceBuilder = new ContainerBuilder();

            var configurationService = new ConfigurationService(configPath);
            var databaseService = new DatabaseService(appDataPath);
            var profileService = new ProfileService(appDataPath);
            var iconService = new IconService(appDataPath);

            IDispatcherService dispatcherService;
            if (isSilentMode)
            {
                dispatcherService = new NoUiDispatcher();
            }
            else
            {
                dispatcherService = new DispatcherService();
            }

            serviceBuilder.RegisterInstance(dispatcherService);

            var dialogService = new DialogService(dispatcherService);
            serviceBuilder.RegisterInstance(dialogService);

            var saveManagerService = new SaveManagerService(appDataPath, configurationService, dialogService);

            var modEngine2Service = new ModEngine2Service(configurationService);

            serviceBuilder.RegisterInstance(configurationService);
            serviceBuilder.RegisterInstance(databaseService);
            serviceBuilder.RegisterInstance(profileService);
            serviceBuilder.RegisterInstance(saveManagerService);
            serviceBuilder.RegisterInstance(modEngine2Service);
            serviceBuilder.RegisterInstance(iconService);
            serviceBuilder.RegisterInstance(
                new ShortcutService(
                    iconService, 
                    dialogService,
                    appDataPath));

            var playManagerService = new PlayManagerService(
                profileService,
                saveManagerService,
                modEngine2Service,
                dialogService);

            serviceBuilder.RegisterInstance(playManagerService);

            var profileManagerSerivce = new ProfileManagerService(
                    databaseService,
                    dispatcherService);

            serviceBuilder.RegisterInstance(profileManagerSerivce);

            var modManagerSerivce = new ModManagerService(
                databaseService,
                dispatcherService,
                profileManagerSerivce,
                dialogService);

            serviceBuilder.RegisterInstance(modManagerSerivce);

            var dllManagerSerivce = new DllManagerService(
                databaseService,
                dispatcherService,
                profileManagerSerivce,
                dialogService);

            serviceBuilder.RegisterInstance(dllManagerSerivce);

            var appVersion = typeof(App).Assembly.GetName().Version ?? new Version(0, 0, 0);
            serviceBuilder.RegisterInstance(appVersion);

            serviceBuilder.RegisterInstance(
                new PackageService(
                    appDataPath,
                    appVersion,
                    dialogService,
                    databaseService, 
                    modManagerSerivce));

            if (!isSilentMode)
            {
                serviceBuilder.RegisterType<HomePageVm>();
                serviceBuilder.RegisterType<ProfilesPageVm>();
                serviceBuilder.RegisterType<ModsPageVm>();
                serviceBuilder.RegisterType<DllsPageVm>();
                serviceBuilder.RegisterType<ProfileEditPageVm>();
                serviceBuilder.RegisterType<ModEditPageVm>();
                serviceBuilder.RegisterType<DllEditPageVm>();
                serviceBuilder.RegisterType<SettingsPageVm>();
                serviceBuilder.RegisterType<HelpPageVm>();

                serviceBuilder.RegisterInstance(new NavigationService());
            }

            return serviceBuilder.Build();
        }

        private static Process? GetOtherInstance()
        {
            try
            {
                var processes = Process.GetProcessesByName("Metis Mod Launcher");
                var currentProcess = Process.GetCurrentProcess();

                Process? otherProcess = null;
                if (processes.Length > 1)
                {
                    otherProcess = processes.FirstOrDefault(x => x.Id != currentProcess.Id);
                }

                foreach (var process in processes)
                {
                    if (otherProcess is null || process.Id != otherProcess.Id)
                    {
                        process.Dispose();
                    }
                }

                return otherProcess;
            }
            catch
            {
                return null;
            }
        }
    }
}
