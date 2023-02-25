using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class HotBarButtonVm : ObservableObject
    {
        public ICommand Command { get; }

        public string Text { get; }

        public PackIconKind PackIcon { get; }

        public HotBarButtonVm(
            string text, 
            PackIconKind icon, 
            Action action, 
            Func<bool>? actionEnabled = null)
        {
            Command = new RelayCommand(action, actionEnabled ?? (() => true));
            PackIcon = icon;
            Text = text;
        }

        public void RaiseNotifyCommandExecuteChanged()
        {
            (Command as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }
}
