using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainPanelVm : ObservableObject
    {
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
            Navigator = navigator;

            Profiles = new ObservableCollection<ProfileVm>(new List<ProfileVm>()
            {
                new ProfileVm(
                    "Elden Ring",
                    "The base game with no mods",
                    Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EldenRing256.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                new ProfileVm(
                    "Limes Limes Limes Limes",
                    "Why can't I hold all these limes! Why can't I hold all these limes! Why can't I hold all these limes!",
                    "C:\\Users\\Thebb\\Pictures\\Saved Pictures\\hold-all-these-limes.jpg",
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                new ProfileVm(
                    "Banderhob",
                    "",
                    "C:\\Users\\Thebb\\Pictures\\banderhobb.PNG",
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                new ProfileVm(
                    "Banderhob",
                    "",
                    "C:\\Users\\Thebb\\Pictures\\banderhobb.PNG",
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                new ProfileVm(
                    "Banderhob",
                    "",
                    "C:\\Users\\Thebb\\Pictures\\banderhobb.PNG",
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                new ProfileVm(
                    "Banderhob",
                    "",
                    "C:\\Users\\Thebb\\Pictures\\banderhobb.PNG",
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                new ProfileVm(
                    "Banderhob",
                    "",
                    "C:\\Users\\Thebb\\Pictures\\banderhobb.PNG",
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond)))
            });

            Mods = new ObservableCollection<ModVm>()
                {
                    new ModVm(
                        "Elden Ring",
                        Path.GetRandomFileName(),
                        "The base game with no mods",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EldenRing256.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Limes Limes Limes Limes",
                        Path.GetRandomFileName(),
                        "Why can't I hold all these limes! Why can't I hold all these limes! Why can't I hold all these limes!",
                        "C:\\Users\\Thebb\\Pictures\\Saved Pictures\\hold-all-these-limes.jpg",
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Banderhob",
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Green.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Banderhob",
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blue.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Limes Limes Limes Limes",
                        Path.GetRandomFileName(),
                        "Why can't I hold all these limes! Why can't I hold all these limes! Why can't I hold all these limes!",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Red.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Banderhob",
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Green.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Banderhob",
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blue.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Zimes Limes Limes Limes",
                        Path.GetRandomFileName(),
                        "Why can't I hold all these limes! Why can't I hold all these limes! Why can't I hold all these limes!",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Red.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Banderhob",
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Green.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Banderhob",
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blue.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new ModVm(
                        "Limes Limes Limes Limes",
                        Path.GetRandomFileName(),
                        "Ahy can't I hold all these limes! Why can't I hold all these limes! Why can't I hold all these limes!",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Red.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond)))
                };

            Dlls = new ObservableCollection<DllVm>()
                {
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "The base game with no mods",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "EldenRing256.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "Why can't I hold all these limes! Why can't I hold all these limes! Why can't I hold all these limes!",
                        "C:\\Users\\Thebb\\Pictures\\Saved Pictures\\hold-all-these-limes.jpg",
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        "Banderhob",
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Green.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blue.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "Why can't I hold all these limes! Why can't I hold all these limes! Why can't I hold all these limes!",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Red.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Green.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blue.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "Why can't I hold all these limes! Why can't I hold all these limes! Why can't I hold all these limes!",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Red.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Green.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Blue.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond))),
                    new DllVm(
                        Path.ChangeExtension(Path.GetRandomFileName(), ".dll"),
                        Path.GetRandomFileName(),
                        "Ahy can't I hold all these limes! Why can't I hold all these limes! Why can't I hold all these limes!",
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Red.png"),
                        DateTime.UtcNow.AddDays(new Random().Next(DateTime.Now.Millisecond)))
                };

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
            var newProfile = new ProfileVm(GetNewName(Profiles));

            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {                
                Profiles.Insert(0, newProfile);
            });

            await Navigator.NavigateTo(new ProfileEditPageVm(newProfile, true));
        }

        private async Task NavigateEditProfile(ProfileVm? profile)
        {
            if (profile is null)
            {
                return;
            }

            await Navigator.NavigateTo(new ProfileEditPageVm(profile, false));
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

            var duplicate = new ProfileVm(
                GetNewName(Profiles, profile.Name),
                profile.Description,
                profile.ImagePath,
                DateTime.Now);

            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                Profiles.Insert(0, duplicate);
            });

            await Navigator.NavigateTo(new ProfileEditPageVm(duplicate, false));
        }

        private async Task DeleteProfile(ProfileVm? profile)
        {
            if (profile is null)
            {
                return;
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Profiles.Remove(profile);
            });

            if(Navigator.CurrentPage is ProfileEditPageVm profileEditPageVm && 
                profileEditPageVm.Profile == profile)
            {
                await NavigateHome();
            }

            Navigator.ClearHistory();
        }

        private async Task NavigateEditMod(ModVm? mod)
        {
            if(mod is null)
            {
                return;
            }

            await Navigator.NavigateTo(new ModEditPageVm(mod, false));
        }

        private async Task DuplicateMod(ModVm? mod)
        {
            if (mod is null)
            {
                return;
            }

            var duplicate = new ModVm(
                "x",
                mod.FolderPath,
                mod.Description,
                mod.ImagePath,
                DateTime.Now);

            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                Mods.Insert(0, duplicate);
            });

            await Navigator.NavigateTo(new ModEditPageVm(duplicate, false));
        }

        private async Task DeleteMod(ModVm? mod)
        {
            if (mod is null)
            {
                return;
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Mods.Remove(mod);
            });

            if (Navigator.CurrentPage is ModEditPageVm modEditPageVm &&
                modEditPageVm.Mod == mod)
            {
                await NavigateHome();
            }

            Navigator.ClearHistory();
        }

        private static string GetNewName(
            IEnumerable<ProfileVm> currentProfiles, 
            string startingName = "New Profile")
        {
            var newName = startingName;
            while (currentProfiles.Any(x => x.Name == newName))
            {
                newName = $"{newName} - Copy";
            }

            return newName;
        }
    }
}
