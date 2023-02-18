using Autofac;
using ModEngine2ConfigTool.Helpers;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using PowerArgs;
using Sherlog;
using Sherlog.Appenders;
using Sherlog.Formatters;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
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
                WindowHelper.BringProcessToFront(otherProcess);
                otherProcess.Dispose();
                return;
            }

            var commandLineArgs = Args.Parse<CommandLineArgs>(args);
            var configPath = commandLineArgs.AppSettings ?? "appsettings.json";

            var appDataPath = commandLineArgs.AppData
                ?? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Metis Mod Launcher");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            ConfigureLogging(appDataPath);

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
            var dispatcherService = new NoUiDispatcher();

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
            var configurationService = new ConfigurationService(configPath);
            var databaseService = new DatabaseService(appDataPath);
            var profileService = new ProfileService(appDataPath);
            var saveManagerService = new SaveManagerService(appDataPath);

            var modEngine2FolderDefault = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..\\ModEngine2\\ModEngine-2.0.0-preview4-win64");

            var modEngine2Folder = !string.IsNullOrWhiteSpace(configurationService.ModEngine2Folder)
                ? configurationService.ModEngine2Folder
                : modEngine2FolderDefault;

            var modEngine2Service = new ModEngine2Service(modEngine2Folder);

            var serviceBuilder = new ContainerBuilder();
            serviceBuilder.RegisterInstance(configurationService);
            serviceBuilder.RegisterInstance(databaseService);
            serviceBuilder.RegisterInstance(profileService);
            serviceBuilder.RegisterInstance(saveManagerService);
            serviceBuilder.RegisterInstance(modEngine2Service);

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

            var playManagerService = new PlayManagerService(
                profileService,
                saveManagerService,
                modEngine2Service,
                dispatcherService);

            serviceBuilder.RegisterInstance(playManagerService);

            var profileManagerSerivce = new ProfileManagerService(
                    databaseService,
                    dispatcherService);

            serviceBuilder.RegisterInstance(profileManagerSerivce);

            var modManagerSerivce = new ModManagerService(
                databaseService,
                dispatcherService,
                profileManagerSerivce);

            serviceBuilder.RegisterInstance(modManagerSerivce);

            var dllManagerSerivce = new DllManagerService(
                databaseService,
                dispatcherService,
                profileManagerSerivce);

            serviceBuilder.RegisterInstance(dllManagerSerivce);

            serviceBuilder.RegisterInstance(
                new PackageService(
                    appDataPath, 
                    dispatcherService,
                    databaseService, 
                    profileManagerSerivce, 
                    modManagerSerivce, 
                    dllManagerSerivce));

            if (!isSilentMode)
            {
                serviceBuilder.RegisterType<HomePageVm>();
                serviceBuilder.RegisterType<ProfilesPageVm>();
                serviceBuilder.RegisterType<ModsPageVm>();
                serviceBuilder.RegisterType<DllsPageVm>();
                serviceBuilder.RegisterType<ProfileEditPageVm>();
                serviceBuilder.RegisterType<ModEditPageVm>();
                serviceBuilder.RegisterType<DllEditPageVm>();
                serviceBuilder.RegisterType<HelpPageVm>();

                serviceBuilder.RegisterInstance(new NavigationService());
            }

            return serviceBuilder.Build();
        }

        private static void ConfigureLogging(string dataStorage)
        {
            var messageFormatter = new LogMessageFormatter();
            var timestampFormatter = new TimestampFormatter(
                () => DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

            var consoleAppender = new ConsoleAppender(new Dictionary<LogLevel, ConsoleColor>
            {
                {LogLevel.Trace, ConsoleColor.Cyan},
                {LogLevel.Debug, ConsoleColor.Blue},
                {LogLevel.Info, ConsoleColor.White},
                {LogLevel.Warn, ConsoleColor.Yellow},
                {LogLevel.Error, ConsoleColor.Red},
                {LogLevel.Fatal, ConsoleColor.Magenta},
            });

            var logFileName = Path.Combine(
                dataStorage,
                DateTime.Now.ToString("yyyy-M-dd_ModEngine2Config" + ".log"));

            var fileAppender = new FileWriterAppender(logFileName);

            Logger.AddAppender((logger, level, message) =>
            {
                message = messageFormatter.FormatMessage(logger, level, message);
                message = timestampFormatter.FormatMessage(logger, level, message);
                consoleAppender.WriteLine(logger, level, message);
                fileAppender.WriteLine(logger, level, message);
            });
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
