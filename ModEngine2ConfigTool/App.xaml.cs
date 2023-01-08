using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels;
using Sherlog;
using Sherlog.Appenders;
using Sherlog.Formatters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace ModEngine2ConfigTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Logger Logger { get; private set; }

        public static ConfigurationService ConfigurationService { get; }

        public const string DialogHostId = "MainWindowDialogHost";

        static App()
        {
            ConfigureLogging();
            Logger = Logger.GetLogger(nameof(App));
            ConfigurationService = new ConfigurationService();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigureLogging();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Get everything the profile service needs setup
            ProfileService.Initialise();

            var mainViewModel = new MainWindowViewModel();
            var mainWindow = new MainWindow(mainViewModel);

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
    }
}
