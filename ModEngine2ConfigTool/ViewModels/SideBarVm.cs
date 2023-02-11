using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.Pages;
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
        private readonly SaveManagerService _saveManagerService;

        public ICommand NavigateHomeCommand { get; }

        public ICommand NavigateProfilesCommand { get; }

        public ICommand NavigateModsCommand { get; }

        public ICommand NavigateExternalDllsCommand { get; }

        //public ICommand NavigateSettingsCommand { get; }

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
            PlayManagerService playManagerService,
            SaveManagerService saveManagerService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _dllManagerService = dllManagerService;
            _playManagerService = playManagerService;
            _saveManagerService = saveManagerService;
            NavigateHomeCommand = new AsyncRelayCommand(NavigateHome);
            NavigateProfilesCommand = new AsyncRelayCommand(NavigateToProfiles);
            NavigateModsCommand = new AsyncRelayCommand(NavigateToMods);
            NavigateExternalDllsCommand = new AsyncRelayCommand(NavigateToDlls);
            //NavigateSettingsCommand = new AsyncRelayCommand(_mainPanelVm.NavigateSettings);
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
                    _modManagerService,
                    _dllManagerService,
                    _playManagerService,
                    _saveManagerService));
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
                    _modManagerService,
                    _dllManagerService,
                    _playManagerService,
                    _saveManagerService));
            }
        }

        private async Task NavigateHome()
        {
            await _navigationService.NavigateTo(
                new HomePageVm(
                    _navigationService,
                    _profileManagerService, 
                    _modManagerService,
                    _dllManagerService,
                    _playManagerService,
                    _saveManagerService));
        }

        private async Task NavigateToProfiles()
        {
            await _navigationService.NavigateTo(
                new ProfilesPageVm(
                    _navigationService,
                    _profileManagerService,
                    _modManagerService,
                    _dllManagerService,
                    _playManagerService,
                    _saveManagerService));
        }

        private async Task NavigateToMods()
        {
            await _navigationService.NavigateTo(
                new ModsPageVm(
                    _navigationService,
                    _profileManagerService,
                    _modManagerService));
        }

        private async Task NavigateToDlls()
        {
            await _navigationService.NavigateTo(
                new DllsPageVm(
                    _navigationService,
                    _profileManagerService,
                    _dllManagerService));
        }

        private async Task NavigateToCreateProfile()
        {
            var profileVm = await _profileManagerService.CreateNewProfileAsync("New Profile");

            var profileEditPageVm = new ProfileEditPageVm(
                profileVm,
                false,
                _navigationService,
                _profileManagerService,
                _modManagerService,
                _dllManagerService,
                _playManagerService,
                _saveManagerService);

            await _navigationService.NavigateTo(profileEditPageVm);
        }

        private async Task NavigateAddMod()
        {
            var modVm = await _modManagerService.ImportModAsync();
            if(modVm is null)
            {
                return;
            }

            var modEditPageVm = new ModEditPageVm(
                modVm,
                false,
                _navigationService,
                _profileManagerService,
                _modManagerService);

            await _navigationService.NavigateTo(modEditPageVm);
        }

        private async Task NavigateAddDll()
        {
            var dllVm = await _dllManagerService.ImportDllAsync();
            if (dllVm is null)
            {
                return;
            }

            var dllEditPageVm = new DllEditPageVm(
                dllVm,
                false,
                _navigationService,
                _profileManagerService,
                _dllManagerService);

            await _navigationService.NavigateTo(dllEditPageVm);
        }

        private async Task NavigateHelp()
        {
            await _navigationService.NavigateTo(new HelpPageVm());
        }
    }
}
