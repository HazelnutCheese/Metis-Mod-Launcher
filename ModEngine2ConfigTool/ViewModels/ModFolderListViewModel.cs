using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.ViewModels.Fields;
using ModEngine2ConfigTool.Views.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModEngine2ConfigTool.ViewModels
{
    public class ModFolderListViewModel : BaseOnDiskListViewModel<ModViewModel>
    {
        private string _lastOpenedLocation;
        public override bool IsChanged => OnDiskObjectList.Any(x => x.IsChanged) || base.IsChanged;

        public ModFolderListViewModel(List<ModViewModel> modList) : base(modList)
        {
            _lastOpenedLocation = string.Empty;
            CanEdit = true;
        }

        protected async override Task Edit()
        {
            if (SelectedItem is null)
            {
                return;
            }

            var dialog = new CustomDialogView();

            var dialogAcceptCommand = new RelayCommand(() =>
            {
                DialogHost.CloseDialogCommand.Execute(true, dialog);
            });

            var dialogVm = new CustomDialogViewModel(
                "Rename Mod",
                "Specify a new name for the mod." +
                "\n\nNote: This is only the name in the profile, it does not make any on disk changes.",
                new List<IFieldViewModel>()
                {
                    new TextFieldViewModel(
                        "New Name:",
                        "Enter the new name for the mod",
                        SelectedItem.Name,
                        new List<ValidationRule<string>>()
                        {
                            new ValidationRule<string>(s => !string.IsNullOrWhiteSpace(s), "The field cannot be empty.")
                        })
                },
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel(
                        "Accept", 
                        CustomDialogViewModel.GetCloseDialogCommand(true, dialog), 
                        isDefault: false),
                    new DialogButtonViewModel(
                        "Cancel", 
                        CustomDialogViewModel.GetCloseDialogCommand(false, dialog), 
                        isDefault: true)
                });

            dialog.DataContext = dialogVm;

            var result = await DialogHost.Show(dialog, App.DialogHostId);

            if(result is not bool || result.Equals(false))
            {
                return;
            }

            SelectedItem.Name = ((TextFieldViewModel) dialogVm.Fields.Fields.Single()).Value;
        }

        protected override void AddNew()
        {
            var dialog = new FolderBrowserEx.FolderBrowserDialog
            {
                Title = "Select a folder",
                InitialFolder = @"C:\",
                AllowMultiSelect = false
            };

            if (_lastOpenedLocation.Equals(string.Empty))
            {
                dialog.InitialFolder = OnDiskObjectList.Any() 
                    ? OnDiskObjectList.First().Location
                    : Directory.GetCurrentDirectory();
            }
            else
            {
                dialog.InitialFolder = _lastOpenedLocation;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _lastOpenedLocation = dialog.SelectedFolder;

                var modName = new DirectoryInfo(dialog.SelectedFolder).Name;

                var newMod = new ModViewModel(modName, dialog.SelectedFolder);

                if (OnDiskObjectList.Any(x => x.Name == modName && x.Location == dialog.SelectedFolder))
                {
                    return;
                }

                newMod.PropertyChanged += Mod_Changed;
                OnDiskObjectList.Add(newMod);
            }
        }
    }
}
