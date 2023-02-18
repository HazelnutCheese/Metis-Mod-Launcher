using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class HomePageVm : ObservableObject
    {
        private readonly ObservableCollection<ProfileVm> _profiles;
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;
        private readonly DllManagerService _dllManagerService;
        private readonly PlayManagerService _playManagerService;
        private readonly SaveManagerService _saveManagerService;
        private ICollectionView _recentProfiles;

        private readonly ObservableCollection<ProfileListButtonVm> _profileListButtons;

        public ICollectionView RecentProfiles 
        { 
            get => _recentProfiles;
            private set => SetProperty(ref _recentProfiles, value);
        }

        public HotBarVm HotBarVm { get; }
        public string BackgroundImage { get; }

        public HomePageVm(
            NavigationService navigationService,
            ProfileManagerService profileManagerService, 
            ModManagerService modManagerService,
            DllManagerService dllManagerService,
            PlayManagerService playManagerService,
            SaveManagerService saveManagerService)
        {
            _profiles = profileManagerService.ProfileVms;

            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _dllManagerService = dllManagerService;
            _playManagerService = playManagerService;
            _saveManagerService = saveManagerService;

            _profileListButtons = new ObservableCollection<ProfileListButtonVm>();
            UpdateProfileListButtons();

            _recentProfiles = CollectionViewSource.GetDefaultView(_profileListButtons
                .OrderByDescending(x => x.LastPlayed)
                .Take(10));

            HotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    new HotBarButtonVm(
                        "Create new Profile",
                        PackIconKind.PencilOutline,
                        async () => await NavigateToCreateProfileAsync()),
                    //new HotBarButtonVm(
                    //    "Import Mod",
                    //    PackIconKind.FolderMultiplePlusOutline,
                    //    async () => await NavigateToImportModAsync()),
                    //new HotBarButtonVm(
                    //    "Import External Dll",
                    //    PackIconKind.FilePlusOutline,
                    //    async () => await NavigateToImportDllAsync())
                });

            BackgroundImage = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Background_04.png");

            _profiles.CollectionChanged += async (s,e) => await UpdateRecentProfiles();
        }

        private async Task UpdateRecentProfiles()
        {
            UpdateProfileListButtons();
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                RecentProfiles = CollectionViewSource.GetDefaultView(_profileListButtons
                    .OrderByDescending(x => x.LastPlayed)
                    .Take(10));
            });
        }

        private async Task NavigateToCreateProfileAsync()
        {
            var profileVm = await _profileManagerService.CreateNewProfileAsync("New Profile");

            await _navigationService.NavigateTo<ProfileEditPageVm>(
                    new NamedParameter("profile", profileVm));
        }

        private async Task NavigateToImportModAsync()
        {
            var modVm = await _modManagerService.ImportModAsync();
            if(modVm is null)
            {
                return;
            }

            await _navigationService.NavigateTo<ModEditPageVm>(
                new NamedParameter("mod", modVm));
        }

        private async Task NavigateToImportDllAsync()
        {
            var dllVm = await _dllManagerService.ImportDllAsync();
            if (dllVm is null)
            {
                return;
            }

            await _navigationService.NavigateTo<DllEditPageVm>(
                new NamedParameter("dll", dllVm));
        }

        private void UpdateProfileListButtons()
        {
            _profileListButtons.Clear();
            foreach (var profile in _profileManagerService.ProfileVms)
            {
                _profileListButtons.Add(new ProfileListButtonVm(
                    profile,
                    _navigationService,
                    _profileManagerService,
                    _modManagerService,
                    _playManagerService));
            }
        }
    }
}
