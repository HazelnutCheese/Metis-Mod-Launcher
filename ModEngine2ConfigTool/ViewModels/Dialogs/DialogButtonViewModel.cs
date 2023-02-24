using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Dialogs
{
    public class DialogButtonViewModel
    {
        public string Label { get; }
        public ICommand Command { get; set; }
        public bool IsDefault { get; }

        public DialogButtonViewModel(
            string label, 
            bool result, 
            bool isDefault) 
        {
            Label = label;
            Command = new RelayCommand<IInputElement>(view =>
            {
                DialogHost.CloseDialogCommand.Execute(result, view);
            });
            IsDefault = isDefault;
        }
    }
}
