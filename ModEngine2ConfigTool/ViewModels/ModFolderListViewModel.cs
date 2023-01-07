using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.Views.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ModEngine2ConfigTool.ViewModels
{
    public class ModFolderListViewModel : ModListViewModel
    {
        private string _lastOpenedLocation;

        public ModFolderListViewModel(IEnumerable<ModViewModel> modList) : base(modList)
        {
            _lastOpenedLocation = string.Empty;
            CanEdit = true;
        }

        protected async override void Edit()
        {
            if(SelectedItem is null)
            {
                return;
            }

            var dialogVm = new TextEntryDialogViewModel("Edit Name", SelectedItem.Name);

            var dialog = new TextEntryDialog()
            {
                DataContext = dialogVm
            };

            var result = await DialogHost.Show(dialog, App.DialogHostId);

            if(result is not bool || result.Equals(false))
            {
                return;
            }

            SelectedItem.Name = dialogVm.FieldValue;
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
                dialog.InitialFolder = ProfileModsList.Any() 
                    ? ProfileModsList.First().Location
                    : Directory.GetCurrentDirectory();
            }
            else
            {
                dialog.InitialFolder = _lastOpenedLocation;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var modName = new DirectoryInfo(dialog.SelectedFolder).Name;

                var newMod = new ModViewModel(modName, dialog.SelectedFolder);

                if (ProfileModsList.Any(x => x.Name == modName && x.Location == dialog.SelectedFolder))
                {
                    return;
                }

                ProfileModsList.Add(newMod);
            }
        }
    }
}
