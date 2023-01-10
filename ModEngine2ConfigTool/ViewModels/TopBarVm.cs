using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Extensions;
using ModEngine2ConfigTool.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class TopBarVm : ObservableObject
    {
        private WindowExtensions.Rect? _oldPosition;
        private readonly NavigationService _navigator;

        public ICommand MinimizeCommand { get; }

        public ICommand MaximizeCommand { get; }

        public ICommand WindowedModeCommand { get; }

        public ICommand CloseWindowCommand { get; }

        public ICommand NavigateBackCommand { get; }

        public ICommand NavigateForwardsCommand { get; }

        public MainWindow Window { get; }

        public TopBarVm(MainWindow window, NavigationService navigator)
        {
            Window = window;
            _navigator = navigator;

            MinimizeCommand = new RelayCommand(Minimize);
            MaximizeCommand = new RelayCommand(Maximize);
            WindowedModeCommand = new RelayCommand(WindowedMode);
            CloseWindowCommand = new RelayCommand(CloseWindow);
            NavigateBackCommand = new AsyncRelayCommand(NavigateBack, () => _navigator.HasHistory);
            NavigateForwardsCommand = new AsyncRelayCommand(NavigateForwards, () => _navigator.HasForwards);

            _navigator.HistoryChanged += _navigator_HistoryChanged;
        }

        private void _navigator_HistoryChanged(object? sender, EventArgs e)
        {
            NavigateBackCommand.NotifyCanExecuteChanged();
            NavigateForwardsCommand.NotifyCanExecuteChanged();
        }

        public void Minimize()
        {
            Window.WindowState = System.Windows.WindowState.Minimized;
        }

        public void WindowedMode()
        {
            Window.WindowState = System.Windows.WindowState.Normal;
            Window.ResizeMode = ResizeMode.CanResize;

            if(_oldPosition is not null)
            {
                Window.Top = Math.Max(_oldPosition.Value.Top, 0);
                Window.Left = Math.Max(_oldPosition.Value.Left, 0);
            }
        }

        public void Maximize()
        {
            _oldPosition = new WindowExtensions.Rect
            {
                Top = (int)Window.Top,
                Left = (int)Window.Left
            };

            Window.WindowState = System.Windows.WindowState.Maximized;
        }

        public void CloseWindow()
        {
            Window.Close();
        }

        private async Task NavigateBack()
        {
            await _navigator.NavigateBack();
        }

        private async Task NavigateForwards()
        {
            await _navigator.NavigateForwards();
        }
    }
}
