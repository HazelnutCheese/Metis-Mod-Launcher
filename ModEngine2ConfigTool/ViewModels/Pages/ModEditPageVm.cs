using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.IO;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class ModEditPageVm : ObservableObject
    {
        private string _lastOpenedLocation;

        public ModVm Mod { get; }

        public string Header { get; }

        public ICommand SelectImageCommand { get; }

        public ModEditPageVm(ModVm mod, bool isCreatingNewMod)
        {
            Mod = mod;

            Header = isCreatingNewMod
                ? "Create new Mod"
                : "Edit Mod";

            SelectImageCommand = new RelayCommand(SelectImage);

            _lastOpenedLocation = string.Empty;
        }

        private void SelectImage()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "All Picture Files(*.bmp;*.jpg;*.gif;*.ico;*.png;*.wdp;*.tiff)|" +
                    "*.BMP;*.JPG;*.GIF;*.ICO;*.PNG;*.WDP;*.TIFF|" +
                    "All files (*.*)|*.*",
                Multiselect = false,
                Title = "Select Profile Image",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (_lastOpenedLocation.Equals(string.Empty))
            {
                fileDialog.InitialDirectory = !string.IsNullOrWhiteSpace(Mod.ImagePath)
                    ? Path.GetDirectoryName(Mod.ImagePath)
                    : Directory.GetCurrentDirectory();
            }
            else
            {
                fileDialog.InitialDirectory = _lastOpenedLocation;
            }

            if (fileDialog.ShowDialog().Equals(true))
            {
                _lastOpenedLocation = Path.GetDirectoryName(fileDialog.FileName) ?? string.Empty;
                Mod.ImagePath = fileDialog.FileName;
            }
        }
    }
}
