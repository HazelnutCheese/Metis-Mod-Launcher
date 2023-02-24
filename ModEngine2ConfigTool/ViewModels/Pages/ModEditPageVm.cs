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
        private readonly NavigationService _navigationService;
        private readonly ModManagerService _modManagerService;
        private readonly PackageService _packageService;
        private readonly DialogService _dialogService;

        public ModVm Mod { get; }

        public ICommand SelectImageCommand { get; }
        public ICommand BrowseCommand { get; }

        public HotBarVm HotBarVm { get; }

        public ModEditPageVm(
            ModVm mod,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService,
            PackageService packageService,
            DialogService dialogService)
        {
            _navigationService = navigationService;
            _modManagerService = modManagerService;
            _packageService = packageService;
            _dialogService = dialogService;

            Mod = mod;

            SelectImageCommand = new RelayCommand(SelectImage);
            BrowseCommand = new RelayCommand(Browse);

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
            var imageFile = _dialogService.ShowOpenFileDialog(
                "Select Mod Image",
                DialogService.AllImageFilter,
                Mod.ImagePath,
                Mod.ImagePath);

            if (File.Exists(imageFile))
            {
                Mod.ImagePath = imageFile;
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
            var folderPath = _dialogService.ShowFolderDialog(
                "Select new Mod folder",
                Mod.FolderPath,
                Mod.FolderPath);

            if(folderPath is not null)
            {
                Mod.FolderPath = folderPath;
            }
        }

        private async Task ExportPackage()
        {
            const string fileExtension = "metismodpkg";

            var saveFilePath = _dialogService.ShowSaveFileDialog(
                "Save Mod Package to",
                $"Profile package Files(*.{fileExtension})|*.{fileExtension}",
                fileExtension,
                defaultFolder: null,
                $"{Mod.Name}.{fileExtension}");

            if (saveFilePath is not null)
            {
                await _packageService.ExportMod(Mod, saveFilePath);
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
