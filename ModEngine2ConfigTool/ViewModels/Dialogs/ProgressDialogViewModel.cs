using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Dialogs
{

    public class ProgressDialogViewModel
    {
        public static ICommand GetCloseDialogCommand(
            bool result,
            ProgressDialogView dialog,
            Func<bool>? canExecute = null)
        {
            if (canExecute is not null)
            {
                return new RelayCommand(() => DialogHost.CloseDialogCommand.Execute(result, dialog), canExecute);
            }
            else
            {
                return new RelayCommand(() => DialogHost.CloseDialogCommand.Execute(result, dialog));
            }
        }

        public string Header { get; }
        public string Message { get; }

        public List<DialogButtonViewModel> Buttons { get; }

        public ProgressDialogViewModel(
            string header,
            string message,
            Task progressTask,
            ProgressDialogView view,
            List<DialogButtonViewModel> buttons) 
        {
            Header = header;
            Message = message;
            Buttons = buttons;

            progressTask.ContinueWith(t => DialogHost.CloseDialogCommand.Execute(true, view));
        }
    }
}
