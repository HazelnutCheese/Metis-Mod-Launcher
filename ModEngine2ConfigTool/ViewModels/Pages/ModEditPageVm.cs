using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class ModEditPageVm : ObservableObject
    {
        private string _lastOpenedLocation;
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;

        public ModVm Mod { get; }

        public string Header { get; }

        public ICommand SelectImageCommand { get; }
        public ICommand BrowseCommand { get; }

        public HotBarVm HotBarVm { get; }

        public ModEditPageVm(
            ModVm mod, 
            bool isCreatingNewMod,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;

            Mod = mod;

            Header = isCreatingNewMod
                ? "Create new Mod"
                : "Edit Mod";

            SelectImageCommand = new RelayCommand(SelectImage);
            BrowseCommand = new RelayCommand(Browse);

            _lastOpenedLocation = string.Empty;

            HotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    new HotBarMenuButtonVm(
                        "Add to Profile",
                        PackIconKind.FolderPlusOutline,
                        _profileManagerService.ProfileVms
                            .Select(x => new HotBarMenuButtonItemVm(
                                x.Name,
                                async ()=> await _profileManagerService.AddModToProfile(x, Mod)))
                            .ToList(),
                        modManagerService.ModVms.Any),
                    new HotBarButtonVm(
                        "Copy Mod",
                        PackIconKind.ContentDuplicate,
                        async () => await NavigateToCopyModAsync()),
                    new HotBarButtonVm(
                        "Delete Mod",
                        PackIconKind.DeleteOutline,
                        async () => await DeleteModAsync())
                });
        }

        private void SelectImage()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
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

        private async Task NavigateToCopyModAsync()
        {
            var modVm = await _modManagerService.DuplicateModAsync(Mod);

            var modEditPage = new ModEditPageVm(
                modVm,
                true,
                _navigationService,
                _profileManagerService,
                _modManagerService);

            await _navigationService.NavigateTo(modEditPage);
        }

        private async Task DeleteModAsync()
        {
            await _modManagerService.RemoveModAsync(Mod);
            await _navigationService.NavigateTo(
                new ModsPageVm(
                    _navigationService, 
                    _profileManagerService, 
                    _modManagerService));
        }

        private void Browse()
        {
            Mod.FolderPath = GetFolderPath("Select new mod folder", Mod.FolderPath) ?? Mod.FolderPath;
        }

        private static string? GetFolderPath(string dialogTitle, string defaultLocation)
        {
            var dialog = new FolderBrowserEx.FolderBrowserDialog
            {
                Title = dialogTitle,
                InitialFolder = @"C:\",
                AllowMultiSelect = false
            };

            if (string.IsNullOrWhiteSpace(defaultLocation) || !Directory.Exists(defaultLocation))
            {
                dialog.InitialFolder = "C:\\";
            }
            else
            {
                dialog.InitialFolder = defaultLocation;
            }

            if (dialog.ShowDialog().Equals(DialogResult.OK))
            {
                return dialog.SelectedFolder;
            }
            else
            {
                return null;
            }
        }
    }
}
