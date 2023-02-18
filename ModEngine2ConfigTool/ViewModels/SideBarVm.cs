using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class SideBarVm : ObservableObject
    {
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;
        private readonly DllManagerService _dllManagerService;
        private readonly PlayManagerService _playManagerService;

        public ICommand NavigateHomeCommand { get; }

        public ICommand NavigateProfilesCommand { get; }

        public ICommand NavigateModsCommand { get; }

        public ICommand NavigateExternalDllsCommand { get; }

        public ICommand NavigateHelpCommand { get; }

        public ICommand NavigateCreateNewProfileCommand { get; }

        public ICommand NavigateAddModCommand { get; }

        public ICommand NavigateAddDllCommand { get; }

        public ObservableCollection<SideBarProfileButtonVm> ProfileButtons { get; }

        public SideBarVm(
            NavigationService navigationService, 
            ProfileManagerService profileManagerService, 
            ModManagerService modManagerService,
            DllManagerService dllManagerService,
            PlayManagerService playManagerService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _dllManagerService = dllManagerService;
            _playManagerService = playManagerService;

            NavigateHomeCommand = new AsyncRelayCommand(NavigateHome);
            NavigateProfilesCommand = new AsyncRelayCommand(NavigateToProfiles);
            NavigateModsCommand = new AsyncRelayCommand(NavigateToMods);
            NavigateExternalDllsCommand = new AsyncRelayCommand(NavigateToDlls);
            NavigateHelpCommand = new AsyncRelayCommand(NavigateHelp);
            NavigateCreateNewProfileCommand = new AsyncRelayCommand(NavigateToCreateProfile);
            NavigateAddModCommand = new AsyncRelayCommand(NavigateAddMod);
            NavigateAddDllCommand = new AsyncRelayCommand(NavigateAddDll);

            _profileManagerService.ProfileVms.CollectionChanged += ProfileVms_CollectionChanged;

            ProfileButtons = new ObservableCollection<SideBarProfileButtonVm>();
            foreach (var profile in _profileManagerService.ProfileVms)
            {
                ProfileButtons.Add(new SideBarProfileButtonVm(
                    profile,
                    _navigationService,
                    _profileManagerService,
                    _playManagerService));
            };
        }

        private void ProfileVms_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ProfileButtons.Clear();
            foreach (var profile in _profileManagerService.ProfileVms)
            {
                ProfileButtons.Add(new SideBarProfileButtonVm(
                    profile,
                    _navigationService,
                    _profileManagerService,
                    _playManagerService));
            }
        }

        private async Task NavigateHome()
        {
            await _navigationService.NavigateTo<HomePageVm>();
        }

        private async Task NavigateToProfiles()
        {
            await _navigationService.NavigateTo<ProfilesPageVm>();
        }

        private async Task NavigateToMods()
        {
            await _navigationService.NavigateTo<ModsPageVm>();
        }

        private async Task NavigateToDlls()
        {
            await _navigationService.NavigateTo<DllsPageVm>();
        }

        private async Task NavigateToCreateProfile()
        {
            var profileVm = await _profileManagerService.CreateNewProfileAsync("New Profile");

            await _navigationService.NavigateTo<ProfileEditPageVm>(
                new NamedParameter("profile", profileVm));
        }

        private async Task NavigateAddMod()
        {
            var modVm = await _modManagerService.ImportModAsync();
            if(modVm is null)
            {
                return;
            }

            await _navigationService.NavigateTo<ModEditPageVm>(
                new NamedParameter("mod", modVm));
        }

        private async Task NavigateAddDll()
        {
            var dllVm = await _dllManagerService.ImportDllAsync();
            if (dllVm is null)
            {
                return;
            }

            await _navigationService.NavigateTo<DllEditPageVm>(
                new NamedParameter("dll", dllVm));
        }

        private async Task NavigateHelp()
        {
            await _navigationService.NavigateTo<HelpPageVm>();
        }
    }
}
