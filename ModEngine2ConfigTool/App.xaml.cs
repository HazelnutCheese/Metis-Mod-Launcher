using ModEngine2ConfigTool.Helpers;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels;
using Sherlog;
using Sherlog.Appenders;
using Sherlog.Formatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ModEngine2ConfigTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Logger Logger { get; private set; }

        public static string DataStorage { get; }

        public static ConfigurationService ConfigurationService { get; }

        public const string DialogHostId = "MainWindowDialogHost";

        static App()
        {
            if(Debugger.IsAttached)
            {
                DataStorage = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ModEngine2ConfigTool");
            }
            else
            {
                DataStorage = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ModEngine2ConfigTool");
            }

            ConfigureLogging();
            Logger = Logger.GetLogger(nameof(App));
            ConfigurationService = new ConfigurationService();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            using var otherProcess = GetOtherInstance();
            if (otherProcess != null)
            {
                WindowHelper.BringProcessToFront(otherProcess);
                otherProcess.Dispose();
                Shutdown();
                return;
            }

            base.OnStartup(e);

            ConfigureLogging();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Initialise Services
            ProfileService.Initialise();
            var mainWindow = new MainWindow();

            var mainViewModel = new MainWindowVm(mainWindow);
            
            mainWindow.DataContext = mainViewModel;

            mainWindow.Show();
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
            Logger.Error(e.Exception.ToString());
            e.SetObserved();
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
            Logger.Error(e.Exception.ToString());
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
            Logger.Error(e.ExceptionObject.ToString());
        }

        private static void ConfigureLogging()
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

            var logFileName = DateTime.Now.ToString("yyyy-M-dd_ModEngine2Config.log");
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
                var processes = Process.GetProcessesByName("ModEngine2ConfigTool");
                var currentProcess = Process.GetCurrentProcess();

                Process? otherProcess = null;
                if (processes.Length > 1)
                {
                    otherProcess = processes.FirstOrDefault(x => x.Id != currentProcess.Id);
                }

                foreach(var process in processes)
                {
                    if(otherProcess is null || process.Id != otherProcess.Id) 
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
