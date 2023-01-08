using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Dialogs
{
    public class TextEntryDialogViewModel : ObservableObject
    {
        private string _fieldValue;

        public string Header { get; }

        public string FieldValue 
        { 
            get => _fieldValue; 
            set => SetProperty(ref _fieldValue, value); 
        }

        public bool HasButton { get; }
        public string ButtonLabel { get; }
        public ICommand? ButtonCommand { get; }

        public TextEntryDialogViewModel(string header, string defaultFieldValue)
        {
            Header = header;
            _fieldValue = defaultFieldValue;
            ButtonLabel = string.Empty;
        }

        public TextEntryDialogViewModel(
            string header, 
            string defaultFieldValue, 
            string buttonLabel, 
            ICommand buttonCommand) : this(header, defaultFieldValue)
        {
            HasButton = true;
            ButtonLabel = buttonLabel;
            ButtonCommand = buttonCommand;
        }
    }
}
