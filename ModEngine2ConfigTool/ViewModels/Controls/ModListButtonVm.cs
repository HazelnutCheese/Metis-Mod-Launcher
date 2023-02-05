using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class ModListButtonMenuItemVm : ObservableObject
    {
        public string Header { get; }

        public ICommand Command { get; }

        public ModListButtonMenuItemVm(string header, Action action)
        {
            Header = header;
            Command = new RelayCommand(action);
        }
    }

    public class ModListButtonVm : ObservableObject
    {
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;

        public ModVm Mod { get; }

        public ICommand Command { get; }

        public ICommand EditCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand RemoveCommand { get; }

        public ObservableCollection<ModListButtonMenuItemVm> MenuItems { get; }

        public string Name => Mod.Name;

        public string Description => Mod.Description;

        public string FolderPath => Mod.FolderPath;

        public DateTime Added => Mod.Added;

        public ModListButtonVm(
            ModVm modVm,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService)
        {
            Mod = modVm;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _navigationService = navigationService;

            Command = new AsyncRelayCommand(NavigateToEditModCommand);
            EditCommand = Command;
            CopyCommand = new AsyncRelayCommand(Copy);
            RemoveCommand = new AsyncRelayCommand(Remove);

            MenuItems = new ObservableCollection<ModListButtonMenuItemVm>();
            foreach(var profile in profileManagerService.ProfileVms)
            {
                MenuItems.Add(
                    new ModListButtonMenuItemVm(
                        profile.Name, 
                        async () => await profileManagerService.AddModToProfile(profile, Mod)));
            }
        }

        private async Task NavigateToEditModCommand()
        {
            var modEditPageVm = new ModEditPageVm(
                Mod,
                false,
                _navigationService,
                _profileManagerService,
                _modManagerService);

            await _navigationService.NavigateTo(modEditPageVm);
        }

        private async Task Copy()
        {
            var newMod = await _modManagerService.DuplicateModAsync(Mod);

            var modEditPageVm = new ModEditPageVm(
                newMod,
                true,
                _navigationService,
                _profileManagerService,
                _modManagerService);

            await _navigationService.NavigateTo(modEditPageVm);
        }

        private async Task Remove()
        {
            await _modManagerService.RemoveModAsync(Mod);
        }
    }
}
