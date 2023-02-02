using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Helpers;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainPanelVm : ObservableObject
    {
        private ProfileManagerService _profileManagerService;
        private ModManagerService _modManagerService;
        private ProfileVm? _selectedItem;

        public NavigationService Navigator { get; }

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateProfilesCommand { get; }
        public ICommand NavigateModsCommand { get; }
        public ICommand NavigateExternalDllsCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand NavigateHelpCommand { get; }

        public ICommand NavigateCreateNewProfileCommand { get; }
        public ICommand NavigateEditProfileCommand { get; }
        public ICommand PlayProfileCommand { get; }
        public ICommand DuplicateProfileCommand { get; }
        public ICommand DeleteProfileCommand { get; }
        public ICommand AddModToProfileCommand { get; }
        public ICommand RemoveModFromProfileCommand { get; }

        public ICommand NavigateAddModCommand { get; }
        public ICommand NavigateEditModCommand { get; }
        public ICommand DuplicateModCommand { get; }
        public ICommand DeleteModCommand { get; }

        public ICommand NavigateAddDllCommand { get; }
        public ICommand NavigateEditDllCommand { get; }
        public ICommand DuplicateDllCommand { get; }
        public ICommand DeleteDllCommand { get; }

        public ObservableCollection<ProfileVm> Profiles { get; private set; }

        public ObservableCollection<ModVm> Mods { get; private set; }

        public ObservableCollection<DllVm> Dlls { get; private set; }

        public ProfileVm? SelectedItem 
        { 
            get => _selectedItem; 
            private set => SetProperty(ref _selectedItem, value); 
        }

        public MainPanelVm(NavigationService navigator)
        {
            _profileManagerService = new ProfileManagerService(
                App.DatabaseService,
                App.DispatcherService);

            _modManagerService = new ModManagerService(
                App.DatabaseService,
                App.DispatcherService,
                _profileManagerService);

            Navigator = navigator;

            Profiles = _profileManagerService.ProfileVms;
            Mods = _modManagerService.ModVms;

            Dlls = new ObservableCollection<DllVm>();

            NavigateHomeCommand = new AsyncRelayCommand(NavigateHome);
            NavigateProfilesCommand = new AsyncRelayCommand(NavigateProfiles);
            NavigateModsCommand = new AsyncRelayCommand(NavigateMods);
            NavigateExternalDllsCommand = new AsyncRelayCommand(NavigateExternalDlls);
            NavigateSettingsCommand = new AsyncRelayCommand(NavigateSettings);
            NavigateHelpCommand = new AsyncRelayCommand(NavigateHelp);

            NavigateCreateNewProfileCommand = new AsyncRelayCommand(NavigateCreateNewProfile);
            NavigateEditProfileCommand = new AsyncRelayCommand<ProfileVm>(NavigateEditProfile);
            PlayProfileCommand = new AsyncRelayCommand<ProfileVm>(PlayProfile);
            DuplicateProfileCommand = new AsyncRelayCommand<ProfileVm>(DuplicateProfile);
            DeleteProfileCommand = new AsyncRelayCommand<ProfileVm>(DeleteProfile);

            AddModToProfileCommand = new AsyncRelayCommand<ProfileVmModVmTuple>(
                AddModToProfile);
            RemoveModFromProfileCommand = new AsyncRelayCommand<ProfileVmModVmTuple>(
                RemoveModFromProfile);

            NavigateAddModCommand = new AsyncRelayCommand(NavigateAddMod);
            NavigateEditModCommand = new AsyncRelayCommand<ModVm>(NavigateEditMod);
            DuplicateModCommand = new AsyncRelayCommand<ModVm>(DuplicateMod);
            DeleteModCommand = new AsyncRelayCommand<ModVm>(DeleteMod);

            Navigator.CurrentPage = new HomePageVm(Profiles);
        }

        public async Task NavigateHome()
        {
            await Navigator.NavigateTo(new HomePageVm(Profiles));
        }

        public async Task NavigateProfiles()
        {
            await Navigator.NavigateTo(new ProfilesPageVm(Profiles));
        }

        public async Task NavigateMods()
        {
            await Navigator.NavigateTo(new ModsPageVm(Mods));
        }

        public async Task NavigateExternalDlls()
        {
            await Navigator.NavigateTo(new DllsPageVm(Dlls));
        }

        public async Task NavigateSettings()
        {
            await Navigator.NavigateTo(new SettingsPageVm());
        }

        public async Task NavigateHelp()
        {
            await Navigator.NavigateTo(new HelpPageVm());
        }

        public async Task NavigateCreateNewProfile()
        {
            var newProfileVm = await _profileManagerService.CreateNewProfileAsync("New Profile");
            await Navigator.NavigateTo(new ProfileEditPageVm(newProfileVm, true, _profileManagerService));
        }

        private async Task NavigateEditProfile(ProfileVm? profile)
        {
            if (profile is null)
            {
                return;
            }

            await Navigator.NavigateTo(new ProfileEditPageVm(profile, false, _profileManagerService));
        }

        private async Task PlayProfile(ProfileVm? profile)
        {
            if (profile is null)
            {
                return;
            }

            Debug.WriteLine($"Playing Profile: {profile.Name}");
        }

        private async Task DuplicateProfile(ProfileVm? profile)
        {
            if (profile is null)
            {
                return;
            }

            var duplicate = await _profileManagerService.DuplicateProfileAsync(profile);
            await Navigator.NavigateTo(new ProfileEditPageVm(duplicate, false, _profileManagerService));
        }

        private async Task DeleteProfile(ProfileVm? profile)
        {
            if (profile is null)
            {
                return;
            }

            await _profileManagerService.RemoveProfileAsync(profile);

            if(Navigator.CurrentPage is ProfileEditPageVm profileEditPageVm && 
                profileEditPageVm.Profile == profile)
            {
                await NavigateHome();
            }

            Navigator.ClearHistory();
        }

        public async Task AddModToProfile(ProfileVmModVmTuple? tuple)
        {
            if(tuple is null)
            {
                return;
            }

            await _profileManagerService.AddModToProfile(
                tuple.ProfileVm, 
                tuple.ModVm);
        }

        public async Task RemoveModFromProfile(ProfileVmModVmTuple? tuple)
        {
            if (tuple is null)
            {
                return;
            }

            await _profileManagerService.RemoveModFromProfile(
                tuple.ProfileVm, 
                tuple.ModVm);
        }

        private async Task NavigateAddMod()
        {
            var modPath = GetFolderPath("Select Mod Folder", "C:\\");
            if(modPath is null)
            {
                return;
            }

            var newMod = await _modManagerService.CreateNewModAsync(modPath);
            await Navigator.NavigateTo(new ModEditPageVm(newMod, true));
        }

        private async Task NavigateEditMod(ModVm? mod)
        {
            if(mod is null)
            {
                return;
            }

            await Navigator.NavigateTo(new ModEditPageVm(mod, false));
        }

        private async Task DuplicateMod(ModVm? modVm)
        {
            if (modVm is null)
            {
                return;
            }

            var newMod = await _modManagerService.DuplicateModAsync(modVm);
            await Navigator.NavigateTo(new ModEditPageVm(newMod, true));
        }

        private async Task DeleteMod(ModVm? modVm)
        {
            if (modVm is null)
            {
                return;
            }

            await _modManagerService.RemoveModAsync(modVm);

            if (Navigator.CurrentPage is ModEditPageVm modEditPageVm &&
                modEditPageVm.Mod == modVm)
            {
                await NavigateHome();
            }

            Navigator.ClearHistory();
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

            if (dialog.ShowDialog() == DialogResult.OK)
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
