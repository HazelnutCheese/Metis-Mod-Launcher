using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class HotBarMenuButtonItemVm : ObservableObject
    {
        public string Header { get; }

        public ICommand Command { get; }

        public HotBarMenuButtonItemVm(
            string header, 
            Action action,
            Func<bool>? actionEnabled = null)
        {
            Header = header;
            Command = new RelayCommand(action, actionEnabled ?? (() => true));
        }
    }
}
