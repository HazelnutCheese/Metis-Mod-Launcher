using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public ICommand SelectImageCommand { get; }

        public ICommand BrowseCommand { get; }

        public HotBarVm HotBarVm { get; }

        public DllEditPageVm(
            DllVm dll, 
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            DllManagerService dllManagerService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _dllManagerService = dllManagerService;

            Dll = dll;

            SelectImageCommand = new RelayCommand(SelectImage);
            BrowseCommand = new RelayCommand(Browse);

            _lastOpenedLocation = string.Empty;

            HotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    new HotBarButtonVm(
                        "Open in Explorer",
                        PackIconKind.OpenInApp,
                        () => OpenExplorer(),
                        () => File.Exists(Dll.FilePath)),
                    //new HotBarMenuButtonVm(
                    //    "Add to Profile",
                    //    PackIconKind.FolderPlusOutline,
                    //    _profileManagerService.ProfileVms
                    //        .Select(x => new HotBarMenuButtonItemVm(
                    //            x.Name,
                    //            async ()=> await _profileManagerService.AddDllToProfile(x, Dll)))
                    //        .ToList(),
                    //    profileManagerService.ProfileVms.Any),
                    new HotBarButtonVm(
                        "Copy Dll",
                        PackIconKind.ContentDuplicate,
                        async () => await NavigateToCopyDllAsync()),
                    new HotBarButtonVm(
                        "Remove Dll",
                        PackIconKind.DeleteOutline,
                        async () => await DeleteDllAsync())
                });

            Dll.PropertyChanged += Dll_PropertyChanged;
        }

        private void Dll_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Dll.FilePath))
            {
                (HotBarVm.Buttons[0] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
            }
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
            var dllVm = await _dllManagerService.CopyDllAsync(Dll);

            await _navigationService.NavigateTo<DllEditPageVm>(
                new NamedParameter("dll", dllVm));
        }

        private async Task DeleteDllAsync()
        {
            await _dllManagerService.RemoveDllAsync(Dll);
            await _navigationService.NavigateTo<DllsPageVm>();
        }

        private void Browse()
        {
            Dll.FilePath = GetFilePath("Select new dll", Dll.FilePath) ?? Dll.FilePath;
        }

        private static string? GetFilePath(string dialogTitle, string defaultLocation)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Dll files (*.dll)|*.dll|All files (*.*)|*.*",
                Multiselect = false,
                Title = "Select External Dll",
                CheckFileExists = true,
                CheckPathExists = true,
                InitialDirectory = defaultLocation
            };

            if (fileDialog.ShowDialog().Equals(true))
            {
                return fileDialog.FileName;
            }

            return null;
        }

        private void OpenExplorer()
        {
            if (!File.Exists(Dll.FilePath))
            {
                return;
            }

            Process.Start("explorer", Path.GetDirectoryName(Dll.FilePath));
        }
    }
}
