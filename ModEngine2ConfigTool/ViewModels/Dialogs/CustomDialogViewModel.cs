using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Extensions;
using ModEngine2ConfigTool.ViewModels.Fields;
using ModEngine2ConfigTool.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Dialogs
{

    public class CustomDialogViewModel
    {
        public string Header { get; }
        public string Message { get; }

        public FieldsCollectionViewModel Fields { get; set; }
        public List<DialogButtonViewModel> Buttons { get; }

        public CustomDialogViewModel(
            string header,
            string message,
            List<IFieldViewModel>? fields = null, 
            List<DialogButtonViewModel>? buttons = null) 
        {
            Fields = new FieldsCollectionViewModel(fields ?? new List<IFieldViewModel>());
            Header = header;
            Message = message;
            Buttons = buttons ?? new List<DialogButtonViewModel>();

            Fields.PropertyChanged += Fields_PropertyChanged;
        }

        private void Fields_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(Equals(e.PropertyName, nameof(IIsChanged.IsChanged)))
            {
                foreach(var button in Buttons)
                {
                    button.Command.NotifyCanExecuteChanged();
                }
            }
        }
    }
}
