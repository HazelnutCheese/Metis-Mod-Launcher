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
            ICommand command, 
            bool isDefault) 
        {
            Label = label;
            Command = command;
            IsDefault = isDefault;
        }
    }
}
