using Autofac;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels;
using Sherlog;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ModEngine2ConfigTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string AppName = "Metis Mod Launcher";
        public const string DialogHostId = "MainWindowDialogHost";

        private readonly Logger _logger;
        private readonly IContainer _serviceContainer;

        public App(IContainer serviceContainer)
        {
            _logger = Logger.GetLogger(typeof(App));
            _serviceContainer = serviceContainer;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            var mainWindow = new MainWindow();
            var mainViewModel = new MainWindowVm(
                mainWindow,
                _serviceContainer);

            mainWindow.DataContext = mainViewModel;

            mainWindow.Show();
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            if(e.Exception?.InnerException is not null)
            {
                MessageBox.Show(e.Exception.InnerException?.Message);
            }
            else
            {
                MessageBox.Show(e.Exception?.Message);
            }

            _logger.Error(e.Exception?.ToString());

            e.SetObserved();
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
            _logger.Error(e.Exception.ToString());
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
            _logger.Error(e.ExceptionObject.ToString());
        }
    }
}
