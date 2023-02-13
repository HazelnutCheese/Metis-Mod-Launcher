using ModEngine2ConfigTool.Helpers;
using ModEngine2ConfigTool.Services;
using PowerArgs;
using Sherlog;
using Sherlog.Appenders;
using Sherlog.Formatters;
using System;
using System.Collections.Generic;
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


            if (commandLineArgs.ProfileId is null)
            {
                var dispatcherService = new DispatcherService();
                Start(
                    databaseService,
                    dispatcherService,
                    profileService,
                    saveManagerService,
                    modEngine2Service);
            }
            else
            {
                var dispatcherService = new NoUiDispatcher();
                StartSilent(
                    commandLineArgs.ProfileId,
                    profileService, 
                    saveManagerService, 
                    modEngine2Service, 
                    dispatcherService, 
                    databaseService);
            }
        }

        private static void Start(
            IDatabaseService databaseService,
            IDispatcherService dispatcherService,
            ProfileService profileService,
            SaveManagerService saveManagerService,
            ModEngine2Service modEngine2Service)
        { 
            var app = new App(
                databaseService,
                dispatcherService,
                saveManagerService,
                modEngine2Service,
                profileService);

            app.InitializeComponent();
            app.Run();
        }

        private static void StartSilent(
            string profileId,
            ProfileService profileService,
            SaveManagerService saveManagerService,
            ModEngine2Service modEngine2Service,
            IDispatcherService dispatcherService,
            IDatabaseService databaseService)
        {
            var playManagerService = new PlayManagerService(
                profileService,
                saveManagerService,
                modEngine2Service,
                dispatcherService);

            var profileManager = new ProfileManagerService(databaseService, dispatcherService);

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
