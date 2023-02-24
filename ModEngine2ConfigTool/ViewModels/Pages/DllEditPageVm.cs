using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class DllEditPageVm : ObservableObject
    {
        private readonly NavigationService _navigationService;
        private readonly DllManagerService _dllManagerService;
        private readonly DialogService _dialogService;

        public DllVm Dll { get; }

        public ICommand SelectImageCommand { get; }

        public ICommand BrowseCommand { get; }

        public HotBarVm HotBarVm { get; }

        public DllEditPageVm(
            DllVm dll, 
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            DllManagerService dllManagerService,
            DialogService dialogService)
        {
            _navigationService = navigationService;
            _dllManagerService = dllManagerService;
            _dialogService = dialogService;

            Dll = dll;

            SelectImageCommand = new RelayCommand(SelectImage);
            BrowseCommand = new RelayCommand(Browse);

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
            var imageFile = _dialogService.ShowOpenFileDialog(
                "Select External Dll Image",
                DialogService.AllImageFilter,
                Dll.ImagePath,
                Dll.ImagePath);

            if (File.Exists(imageFile))
            {
                Dll.ImagePath = imageFile;
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
            var filePath = _dialogService.ShowOpenFileDialog(
                "Select new Mod folder",
                 "Dll files (*.dll)|*.dll|All files (*.*)|*.*",
                Dll.FilePath,
                Dll.FilePath);

            if (filePath is not null)
            {
                Dll.FilePath = filePath;
            }
        }

        private void OpenExplorer()
        {
            if (File.Exists(Dll.FilePath) && 
                Path.GetDirectoryName(Dll.FilePath) is string parentFolder 
                && Directory.Exists(parentFolder))
            {
                Process.Start("explorer", parentFolder);
            }            
        }
    }
}
