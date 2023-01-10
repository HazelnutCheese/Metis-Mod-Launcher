using ModEngine2ConfigTool.Extensions;
using ModEngine2ConfigTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ModEngine2ConfigTool.Views
{
    public partial class TopBar : UserControl
    {
        private MainWindow? _mainWindow;

        public TopBar()
        {
            InitializeComponent();
            DataContextChanged += TopBar_DataContextChanged;
        }

        private void TopBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(DataContext is TopBarVm topBarViewModel)
            {
                _mainWindow = topBarViewModel.Window;
                _mainWindow.StateChanged += MainWindow_StateChanged;

                SwapButtonVisibility();
            }
        }

        private void MainWindow_StateChanged(object? sender, System.EventArgs e)
        {
            SwapButtonVisibility();
        }

        private void SwapButtonVisibility()
        {
            if (_mainWindow is not null && _mainWindow.WindowState.Equals(WindowState.Normal))
            {
                MaximizeButton.Visibility = Visibility.Visible;
                RestoreButton.Visibility = Visibility.Hidden;
            }

            if (_mainWindow is not null && _mainWindow.WindowState.Equals(WindowState.Maximized))
            {
                MaximizeButton.Visibility = Visibility.Hidden;
                RestoreButton.Visibility = Visibility.Visible;

                //_mainWindow.Maximize();
            }
        }
    }
}
