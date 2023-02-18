using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly PackageService _packageService;

        public ModVm Mod { get; }

        public ICommand SelectImageCommand { get; }
        public ICommand BrowseCommand { get; }

        public HotBarVm HotBarVm { get; }

        public ModEditPageVm(
            ModVm mod,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService,
            PackageService packageService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _packageService = packageService;
            Mod = mod;

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
                        () => Directory.Exists(Mod.FolderPath)),
                    new HotBarButtonVm(
                        "Copy Mod",
                        PackIconKind.ContentDuplicate,
                        async () => await NavigateToCopyModAsync()),
                    new HotBarButtonVm(
                        "Remove Mod",
                        PackIconKind.DeleteOutline,
                        async () => await DeleteModAsync()),
                    new HotBarButtonVm(
                        "Export Package",
                        PackIconKind.PackageVariantClosed,
                        async () => await ExportPackage()),
                });

            Mod.PropertyChanged += Mod_PropertyChanged;
        }

        private void Mod_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Mod.FolderPath))
            {
                (HotBarVm.Buttons[0] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
            }
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

            await _navigationService.NavigateTo<ModEditPageVm>(
                new NamedParameter("mod", modVm));
        }

        private async Task DeleteModAsync()
        {
            await _modManagerService.RemoveModAsync(Mod);
            await _navigationService.NavigateTo<ModsPageVm>();
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

        private async Task ExportPackage()
        {
            const string fileExtension = "metismodpkg";

            var fileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = $"Profile package Files(*.{fileExtension})|*.{fileExtension}",
                Title = "Save Package File",
                AddExtension = true,
                FileName = $"{Mod.Name}.{fileExtension}"
            };

            if (fileDialog.ShowDialog().Equals(true))
            {
                await _packageService.ExportMod(Mod, fileDialog.FileName);
            }
        }

        private void OpenExplorer()
        {
            if(!Directory.Exists(Mod.FolderPath))
            {
                return;
            }

            Process.Start("explorer", Mod.FolderPath);
        }
    }
}
