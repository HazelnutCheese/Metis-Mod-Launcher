using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Extensions;
using ModEngine2ConfigTool.ViewModels.Fields;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        public string PageName { get; } = nameof(SettingsViewModel);

        public FieldsCollectionViewModel Fields { get; }

        public ICommand SaveChangesCommand { get; }

        public ICommand RevertChangesCommand { get; }

        public bool HasErrors => Fields.Fields.Any(
            x => x is INotifyDataErrorInfo validatingField 
                && validatingField.HasErrors);

        public SettingsViewModel()
        {
            var eldenRingPathField = new TextFieldViewModel(
                "Elden Ring Game Folder:", 
                "The path to the Game folder inside your Elden Ring install.", 
                App.ConfigurationService.EldenRingGameFolder, 
                "Browse", 
                new RelayCommand(GetEldenRingGameFolder),
                new List<ValidationRule<string>>()
                {
                    CommonValidationRules.NotEmpty("This field is required."),
                    CommonValidationRules.DirectoryExists(),
                    new ValidationRule<string>(s => s.EndsWith("Game"), "Expected path ending in \"Game\".")
                });

            var modEngine2PathField = new TextFieldViewModel(
                "ModEngine2 Folder:", 
                "The path to the ModEngine2 folder.", 
                App.ConfigurationService.ModEngine2Folder, 
                "Browse", 
                new RelayCommand(GetModEngine2Folder),
                new List<ValidationRule<string>>()
                {
                    CommonValidationRules.NotEmpty("This field is required."),
                    CommonValidationRules.DirectoryExists()
                });

            Fields =new FieldsCollectionViewModel(new List<IFieldViewModel>()
            {
                eldenRingPathField,
                modEngine2PathField
            });

            Fields.PropertyChanged += Fields_PropertyChanged;

            SaveChangesCommand = new RelayCommand(
                SaveChanges, 
                () => Fields.IsChanged 
                    && !Fields.Fields.Any(
                        x => x is INotifyDataErrorInfo validatingField && validatingField.HasErrors));

            RevertChangesCommand = new RelayCommand(
                RevertChanges,
                () => Fields.IsChanged);
        }

        private void Fields_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (Equals(e.PropertyName, nameof(IIsChanged.IsChanged)))
            {
                SaveChangesCommand.NotifyCanExecuteChanged();
                RevertChangesCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(HasErrors));
            }
        }

        private void SaveChanges()
        {
            App.ConfigurationService.EldenRingGameFolder = ((TextFieldViewModel)Fields.Fields[0]).Value;
            App.ConfigurationService.ModEngine2Folder = ((TextFieldViewModel)Fields.Fields[1]).Value;
            Fields.AcceptChanges();
        }

        private void RevertChanges()
        {
            ((TextFieldViewModel)Fields.Fields[0]).Value = App.ConfigurationService.EldenRingGameFolder;
            ((TextFieldViewModel)Fields.Fields[1]).Value = App.ConfigurationService.ModEngine2Folder;
        }

        private void GetEldenRingGameFolder()
        {
            var field = (TextFieldViewModel)Fields.Fields[0];

            var eldenRingGameFolder = GetFolderPath(
                "Select Elden Ring Game Folder",
                field.Value);

            field.Value = eldenRingGameFolder;
        }

        private void GetModEngine2Folder()
        {
            var field = (TextFieldViewModel)Fields.Fields[1];

            var eldenRingGameFolder = GetFolderPath(
                "Select ModEngine2 Folder",
                field.Value);

            field.Value = eldenRingGameFolder;
        }

        private static string GetFolderPath(string dialogTitle, string defaultLocation)
        {
            var dialog = new FolderBrowserEx.FolderBrowserDialog
            {
                Title = dialogTitle,
                InitialFolder = @"C:\",
                AllowMultiSelect = false
            };

            if(string.IsNullOrWhiteSpace(defaultLocation) || !Directory.Exists(defaultLocation)) 
            {
                dialog.InitialFolder = "C:\\";
            }
            else
            {
                dialog.InitialFolder = defaultLocation;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedFolder;
            }
            else
            {
                return defaultLocation;
            }
        }
    }
}
