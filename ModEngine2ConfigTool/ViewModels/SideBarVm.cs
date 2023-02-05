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

        public ICommand NavigateHomeCommand { get; }

        public ICommand NavigateProfilesCommand { get; }

        public ICommand NavigateModsCommand { get; }

        //public ICommand NavigateExternalDllsCommand { get; }

        //public ICommand NavigateSettingsCommand { get; }

        //public ICommand NavigateHelpCommand { get; }

        public ICommand NavigateCreateNewProfileCommand { get; }

        public ObservableCollection<SideBarProfileButtonVm> ProfileButtons { get; }

        public SideBarVm(
            NavigationService navigationService, 
            ProfileManagerService profileManagerService, 
            ModManagerService modManagerService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;

            NavigateHomeCommand = new AsyncRelayCommand(NavigateHome);
            NavigateProfilesCommand = new AsyncRelayCommand(NavigateToProfiles);
            NavigateModsCommand = new AsyncRelayCommand(NavigateToMods);
            //NavigateExternalDllsCommand = new AsyncRelayCommand(NavigateToDlls);
            //NavigateSettingsCommand = new AsyncRelayCommand(_mainPanelVm.NavigateSettings);
            //NavigateHelpCommand = new AsyncRelayCommand(_mainPanelVm.NavigateHelp);
            NavigateCreateNewProfileCommand = new AsyncRelayCommand(NavigateToCreateProfile);

            _profileManagerService.ProfileVms.CollectionChanged += ProfileVms_CollectionChanged;

            ProfileButtons = new ObservableCollection<SideBarProfileButtonVm>();
            foreach (var profile in _profileManagerService.ProfileVms)
            {
                ProfileButtons.Add(new SideBarProfileButtonVm(
                    profile,
                    _navigationService,
                    _profileManagerService,
                    _modManagerService));
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
                    _modManagerService));
            }
        }

        private async Task NavigateHome()
        {
            await _navigationService.NavigateTo(
                new HomePageVm(
                    _navigationService,
                    _profileManagerService, 
                    _modManagerService));
        }

        private async Task NavigateToProfiles()
        {
            await _navigationService.NavigateTo(
                new ProfilesPageVm(
                    _navigationService,
                    _profileManagerService,
                    _modManagerService));
        }

        private async Task NavigateToMods()
        {
            await _navigationService.NavigateTo(
                new ModsPageVm(
                    _navigationService,
                    _profileManagerService,
                    _modManagerService));
        }

        private async Task NavigateToCreateProfile()
        {
            var profileVm = await _profileManagerService.CreateNewProfileAsync("New Profile");

            var profileEditPageVm = new ProfileEditPageVm(
                profileVm,
                false,
                _navigationService,
                _profileManagerService,
                _modManagerService);

            await _navigationService.NavigateTo(profileEditPageVm);
        }
    }
}
