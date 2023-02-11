using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class SideBarProfileButtonVm
    {
        private readonly ProfileVm _profileVm;
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;
        private readonly DllManagerService _dllManagerService;
        private readonly PlayManagerService _playManagerService;
        private readonly SaveManagerService _saveManagerService;
        private readonly PlayManagerService playManagerService;

        public ICommand Command { get; }

        public ICommand PlayCommand { get; }

        public ICommand EditCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand DeleteCommand { get; }

        public string Name => _profileVm.Name;

        public SideBarProfileButtonVm(
            ProfileVm profileVm,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService,
            DllManagerService dllManagerService,
            PlayManagerService playManagerService,
            SaveManagerService saveManagerService)
        {
            _profileVm = profileVm;
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _dllManagerService = dllManagerService;
            _playManagerService = playManagerService;
            _saveManagerService = saveManagerService;

            Command = new AsyncRelayCommand(async () =>
            {
                var profileEditPageVm = new ProfileEditPageVm(
                    profileVm,
                    false,
                    navigationService,
                    profileManagerService,
                    modManagerService,
                    dllManagerService,
                    playManagerService,
                    _saveManagerService);

                await navigationService.NavigateTo(profileEditPageVm);
            });

            PlayCommand = new AsyncRelayCommand(PlayAsync);
            EditCommand = Command;
            CopyCommand = new AsyncRelayCommand(DuplicateProfileAsync);
            DeleteCommand = new AsyncRelayCommand(DeleteProfileAsync);
        }

        private async Task DuplicateProfileAsync()
        {
            var newProfile = await _profileManagerService.DuplicateProfileAsync(_profileVm);
            await _navigationService.NavigateTo(new ProfileEditPageVm(
                newProfile,
                true,
                _navigationService,
                _profileManagerService,
                _modManagerService,
                _dllManagerService,
                _playManagerService,
                _saveManagerService));
        }

        private async Task DeleteProfileAsync()
        {
            await _profileManagerService.RemoveProfileAsync(_profileVm);
            await _navigationService.NavigateTo(
                new ProfilesPageVm(
                    _navigationService,
                    _profileManagerService,
                    _modManagerService,
                    _dllManagerService,
                    _playManagerService,
                    _saveManagerService));
        }

        private async Task PlayAsync()
        {
            await _playManagerService.Play(_profileVm);
        }
    }
}
