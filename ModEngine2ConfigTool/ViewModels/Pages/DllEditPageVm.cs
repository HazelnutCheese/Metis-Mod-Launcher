using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class DllEditPageVm : ObservableObject
    {
        private string _lastOpenedLocation;
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly DllManagerService _dllManagerService;

        public DllVm Dll { get; }

        public string Header { get; }

        public ICommand SelectImageCommand { get; }

        public HotBarVm HotBarVm { get; }

        public DllEditPageVm(
            DllVm dll, 
            bool isCreatingNewMod,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            DllManagerService dllManagerService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _dllManagerService = dllManagerService;

            Dll = dll;

            Header = isCreatingNewMod
                ? "Create new Mod"
                : "Edit Mod";

            SelectImageCommand = new RelayCommand(SelectImage);

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
                                async ()=> await _profileManagerService.AddDllToProfile(x, Dll)))
                            .ToList(),
                        profileManagerService.ProfileVms.Any),
                    new HotBarButtonVm(
                        "Copy Dll",
                        PackIconKind.ContentDuplicate,
                        async () => await NavigateToCopyDllAsync()),
                    new HotBarButtonVm(
                        "Delete Dll",
                        PackIconKind.DeleteOutline,
                        async () => await DeleteDllAsync())
                });
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
                fileDialog.InitialDirectory = !string.IsNullOrWhiteSpace(Dll.ImagePath)
                    ? Path.GetDirectoryName(Dll.ImagePath)
                    : Directory.GetCurrentDirectory();
            }
            else
            {
                fileDialog.InitialDirectory = _lastOpenedLocation;
            }

            if (fileDialog.ShowDialog().Equals(true))
            {
                _lastOpenedLocation = Path.GetDirectoryName(fileDialog.FileName) ?? string.Empty;
                Dll.ImagePath = fileDialog.FileName;
            }
        }

        private async Task NavigateToCopyDllAsync()
        {
            var modVm = await _dllManagerService.CopyDllAsync(Dll);

            var modEditPage = new DllEditPageVm(
                modVm,
                true,
                _navigationService,
                _profileManagerService,
                _dllManagerService);

            await _navigationService.NavigateTo(modEditPage);
        }

        private async Task DeleteDllAsync()
        {
            await _dllManagerService.RemoveDllAsync(Dll);
            await _navigationService.NavigateTo(
                new DllsPageVm(
                    _navigationService, 
                    _profileManagerService, 
                    _dllManagerService));
        }
    }
}
