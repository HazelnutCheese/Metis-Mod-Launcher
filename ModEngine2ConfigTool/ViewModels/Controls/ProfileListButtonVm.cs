using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class ProfileListButtonMenuItemVm : ObservableObject
    {
        public string Header { get; }

        public ICommand Command { get; }

        public ProfileListButtonMenuItemVm(string header, Action action)
        {
            Header = header;
            Command = new RelayCommand(action);
        }
    }

    public class ProfileListButtonVm : ObservableObject
    {
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;
        private readonly PlayManagerService _playManagerService;

        public ProfileVm Profile { get; }

        public ICommand Command { get; }

        public ICommand PlayCommand { get; }

        public ICommand EditCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand DeleteCommand { get; }

        public ObservableCollection<ProfileListButtonMenuItemVm> MenuItems { get; }

        public string Name => Profile.Name;

        public string Description => Profile.Description;

        public DateTime Created => Profile.Created;

        public DateTime? LastPlayed => Profile.LastPlayed;

        public bool CanAddMods => _modManagerService.ModVms.Any();

        public ProfileListButtonVm(
            ProfileVm profileVm,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService,
            PlayManagerService playManagerService)
        {
            Profile = profileVm;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _navigationService = navigationService;
            _playManagerService= playManagerService;

            Command = new AsyncRelayCommand(NavigateToEditModCommand);
            PlayCommand = new AsyncRelayCommand(PlayAsync);
            EditCommand = Command;
            CopyCommand = new AsyncRelayCommand(Copy);
            DeleteCommand = new AsyncRelayCommand(Delete);

            MenuItems = new ObservableCollection<ProfileListButtonMenuItemVm>();
            foreach(var mod in modManagerService.ModVms)
            {
                MenuItems.Add(
                    new ProfileListButtonMenuItemVm(
                        mod.Name, 
                        async () => await profileManagerService.AddModToProfile(Profile, mod)));
            }

            _modManagerService.ModVms.CollectionChanged += ModVms_CollectionChanged;
        }

        private void ModVms_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CanAddMods));
        }

        private async Task NavigateToEditModCommand()
        {
            await _navigationService.NavigateTo<ProfileEditPageVm>(
                new NamedParameter("profile", Profile));
        }

        private async Task Copy()
        {
            var newProfile = await _profileManagerService.DuplicateProfileAsync(Profile);

            await _navigationService.NavigateTo<ProfileEditPageVm>(
                new NamedParameter("profile", newProfile));
        }

        private async Task Delete()
        {
            await _profileManagerService.RemoveProfileAsync(Profile);
        }

        private async Task PlayAsync()
        {
            await _playManagerService.Play(Profile);
        }
    }
}
