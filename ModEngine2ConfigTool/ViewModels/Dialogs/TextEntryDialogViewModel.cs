using CommunityToolkit.Mvvm.ComponentModel;

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

        public TextEntryDialogViewModel(string header, string defaultFieldValue)
        {
            Header = header;
            _fieldValue = defaultFieldValue;
        }
    }
}
