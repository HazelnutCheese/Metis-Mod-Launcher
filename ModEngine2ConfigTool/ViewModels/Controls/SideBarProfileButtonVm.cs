using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
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
    public class SideBarProfileButtonVm : ObservableObject
    {
        private readonly ProfileVm _profileVm;
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly PlayManagerService _playManagerService;

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
            PlayManagerService playManagerService)
        {
            _profileVm = profileVm;
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _playManagerService = playManagerService;

            Command = new AsyncRelayCommand(async () =>
            {
                await _navigationService.NavigateTo<ProfileEditPageVm>(
                    new NamedParameter("profile", profileVm));
            });

            PlayCommand = new AsyncRelayCommand(PlayAsync);
            EditCommand = Command;
            CopyCommand = new AsyncRelayCommand(DuplicateProfileAsync);
            DeleteCommand = new AsyncRelayCommand(DeleteProfileAsync);

            _profileVm.PropertyChanged += _profileVm_PropertyChanged;
        }

        private void _profileVm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ProfileVm.Name))
            {
                OnPropertyChanged(nameof(Name));
            }
        }

        private async Task DuplicateProfileAsync()
        {
            var newProfile = await _profileManagerService.DuplicateProfileAsync(_profileVm);

            await _navigationService.NavigateTo<ProfileEditPageVm>(
                new NamedParameter("profile", newProfile));
        }

        private async Task DeleteProfileAsync()
        {
            await _profileManagerService.RemoveProfileAsync(_profileVm);
            await _navigationService.NavigateTo<ProfilesPageVm>();
        }

        private async Task PlayAsync()
        {
            await _playManagerService.Play(_profileVm);
        }
    }
}
