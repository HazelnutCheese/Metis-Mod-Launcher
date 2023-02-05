using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Helpers;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using ModEngine2ConfigTool.Views.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class ProfileEditPageVm : ObservableObject
    {
        private string _lastOpenedLocation;
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;

        public ProfileVm Profile { get; }

        public string Header { get; }

        public ICommand SelectImageCommand { get; }

        public ObservableCollection<ProfileModListButtonVm> Mods { get; }

        public ICollectionView ExternalDlls { get; }

        public HotBarVm HotBarVm { get; }

        public ProfileEditPageVm(
            ProfileVm profile, 
            bool IsCreatingNewProfile,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService)
        {
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _navigationService = navigationService;

            Profile = profile;
            Header = IsCreatingNewProfile
                ? "Profile"
                : "Profile";

            SelectImageCommand = new RelayCommand(SelectImage);

            _lastOpenedLocation = string.Empty;

            HotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    new HotBarButtonVm(
                        "Play Profile",
                        PackIconKind.PlayBoxOutline,
                        () => Debug.Print($"Playing {profile.Name}"), Profile.Mods.Any),
                    new HotBarMenuButtonVm(
                        "Select Mods",
                        PackIconKind.FolderMultiplePlusOutline,
                        modManagerService.ModVms
                            .Select(x => new HotBarMenuButtonItemVm(
                                x.Name,
                                async ()=> await profileManagerService.AddModToProfile(profile, x)))
                            .ToList(),
                        modManagerService.ModVms.Any),
                    new HotBarButtonVm(
                        "Copy Profile",
                        PackIconKind.ContentDuplicate,
                        async () => await DuplicateProfileAsync()),
                    new HotBarButtonVm(
                        "Delete Profile",
                        PackIconKind.DeleteOutline,
                        async () => await DeleteProfileAsync())
                    });

            Mods = new ObservableCollection<ProfileModListButtonVm>(Profile.Mods.Select(x => new ProfileModListButtonVm(
                profile,
                x,
                navigationService,
                profileManagerService,
                modManagerService)));

            ExternalDlls = CollectionViewSource.GetDefaultView(Profile.ExternalDlls);

            profile.Mods.CollectionChanged += ProfileMods_CollectionChanged;
            modManagerService.ModVms.CollectionChanged += AllMods_CollectionChanged;
        }

        private void ProfileMods_CollectionChanged(
            object? sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Mods.Clear();
            foreach (var mod in Profile.Mods)
            {
                Mods.Add(new ProfileModListButtonVm(
                    Profile,
                    mod,
                    _navigationService,
                    _profileManagerService,
                    _modManagerService));
            }

            // Play Profile Button
            (HotBarVm.Buttons[0] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
        }

        private void AllMods_CollectionChanged(
            object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Add Mods Button
            (HotBarVm.Buttons[1] as HotBarMenuButtonVm)?.RaiseNotifyCommandExecuteChanged();
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
                fileDialog.InitialDirectory = !string.IsNullOrWhiteSpace(Profile.ImagePath)
                    ? Path.GetDirectoryName(Profile.ImagePath)
                    : Directory.GetCurrentDirectory();
            }
            else
            {
                fileDialog.InitialDirectory = _lastOpenedLocation;
            }

            if (fileDialog.ShowDialog().Equals(true))
            {
                _lastOpenedLocation = Path.GetDirectoryName(fileDialog.FileName) ?? string.Empty;
                Profile.ImagePath = fileDialog.FileName;
            }
        }

        private async Task DuplicateProfileAsync()
        {
            var newProfile = await _profileManagerService.DuplicateProfileAsync(Profile);
            await _navigationService.NavigateTo(new ProfileEditPageVm(
                newProfile,
                true,
                _navigationService,
                _profileManagerService,
                _modManagerService));
        }

        private async Task DeleteProfileAsync()
        {
            await _profileManagerService.RemoveProfileAsync(Profile);
            await _navigationService.NavigateTo(
                new ProfilesPageVm(
                    _navigationService,
                    _profileManagerService,
                    _modManagerService));
        }
    }
}
