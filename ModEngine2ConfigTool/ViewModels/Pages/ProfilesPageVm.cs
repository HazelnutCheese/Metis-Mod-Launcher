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
using ModEngine2ConfigTool.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class ProfilesPageVm : ObservableObject
    {
        private ICollectionView _profiles;
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;
        private readonly DllManagerService _dllManagerService;
        private readonly PlayManagerService _playManagerService;
        private readonly SaveManagerService _saveManagerService;
        private readonly PackageService _packageService;
        private readonly ObservableCollection<ProfileListButtonVm> _profileListButtons;

        public ICollectionView Profiles
        {
            get => _profiles;
            set => SetProperty(ref _profiles, value);
        }

        public ICommand SortByNameCommand { get; }

        public ICommand SortByDescriptionCommand { get; }

        public ICommand SortByLastPlayedCommand { get; }

        public ICommand SortByCreatedCommand { get; }

        public HotBarVm HotBarVm { get; }

        public string BackgroundImage { get; }

        public ProfilesPageVm(
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService,
            DllManagerService dllManagerService,
            PlayManagerService playManagerService,
            SaveManagerService saveManagerService,
            PackageService packageService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _dllManagerService = dllManagerService;
            _playManagerService = playManagerService;
            _saveManagerService = saveManagerService;
            _packageService = packageService;

            _profileListButtons = new ObservableCollection<ProfileListButtonVm>();
            UpdateProfileListButtons();

            _profiles = CollectionViewSource.GetDefaultView(_profileListButtons);
            _profiles.SortDescriptions.Add(new SortDescription(
                nameof(ProfileListButtonVm.Name), 
                ListSortDirection.Descending));

            profileManagerService.ProfileVms.CollectionChanged += Profiles_CollectionChanged;

            SortByNameCommand = new AsyncRelayCommand<SortButtonMode>(SortByName);
            SortByDescriptionCommand = new AsyncRelayCommand<SortButtonMode>(SortByDescription);
            SortByLastPlayedCommand = new AsyncRelayCommand<SortButtonMode>(SortByLastPlayed);
            SortByCreatedCommand = new AsyncRelayCommand<SortButtonMode>(SortByCreated);

            HotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    new HotBarButtonVm(
                        "Create new Profile",
                        PackIconKind.PencilOutline,
                        async () => await NavigateToCreateProfileAsync()),
                    //new HotBarButtonVm(
                    //    "Add from Package",
                    //    PackIconKind.PackageVariantPlus,
                    //    async () => await ImportPackage())
                });

            BackgroundImage = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Background_01.png");
        }

        private void Profiles_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateProfileListButtons();
            _profiles.Refresh();
        }

        private async Task SortByName(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileListButtonVm.Name), 
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileListButtonVm.Name), 
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortByDescription(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileListButtonVm.Description),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileListButtonVm.Description),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortByLastPlayed(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileListButtonVm.LastPlayed),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileListButtonVm.LastPlayed),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortByCreated(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileListButtonVm.Created),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileListButtonVm.Created),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task NavigateToCreateProfileAsync()
        {
            var profileVm = await _profileManagerService.CreateNewProfileAsync("New Profile");

            await _navigationService.NavigateTo<ProfileEditPageVm>(
                    new NamedParameter("profile", profileVm));
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

        private async Task ImportPackage()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Profile Package files (*.metispropkg)|*.metispropkg",
                Multiselect = false,
                Title = "Select Profile Package",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (fileDialog.ShowDialog().Equals(true))
            {
                var profileVm = await _packageService.ImportProfile(fileDialog.FileName);

                await _navigationService.NavigateTo<ProfileEditPageVm>(
                    new NamedParameter("profile", profileVm));
            }
        }
    }
}
