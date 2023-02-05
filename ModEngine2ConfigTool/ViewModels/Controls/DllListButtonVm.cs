using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class DllListButtonMenuItemVm : ObservableObject
    {
        public string Header { get; }

        public ICommand Command { get; }

        public DllListButtonMenuItemVm(string header, Action action)
        {
            Header = header;
            Command = new RelayCommand(action);
        }
    }

    public class DllListButtonVm : ObservableObject
    {
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly DllManagerService _dllManagerService;

        public DllVm Dll { get; }

        public ICommand Command { get; }

        public ICommand EditCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand RemoveCommand { get; }

        public ObservableCollection<DllListButtonMenuItemVm> MenuItems { get; }

        public string Name => Dll.Name;

        public string Description => Dll.Description;

        public string FilePath => Dll.FilePath;

        public DateTime Added => Dll.Added;

        public DllListButtonVm(
            DllVm dllVM,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            DllManagerService dllManagerService)
        {
            Dll = dllVM;
            _profileManagerService = profileManagerService;
            _dllManagerService = dllManagerService;
            _navigationService = navigationService;

            Command = new AsyncRelayCommand(NavigateToEditModCommand);
            EditCommand = Command;
            CopyCommand = new AsyncRelayCommand(Copy);
            RemoveCommand = new AsyncRelayCommand(Remove);

            MenuItems = new ObservableCollection<DllListButtonMenuItemVm>();
            foreach(var profile in profileManagerService.ProfileVms)
            {
                MenuItems.Add(
                    new DllListButtonMenuItemVm(
                        profile.Name, 
                        async () => await profileManagerService.AddDllToProfile(profile, Dll)));
            }
        }

        private async Task NavigateToEditModCommand()
        {
            var modEditPageVm = new DllEditPageVm(
                Dll,
                false,
                _navigationService,
                _profileManagerService,
                _dllManagerService);

            await _navigationService.NavigateTo(modEditPageVm);
        }

        private async Task Copy()
        {
            var newDll = await _dllManagerService.CopyDllAsync(Dll);

            var modEditPageVm = new DllEditPageVm(
                newDll,
                true,
                _navigationService,
                _profileManagerService,
                _dllManagerService);

            await _navigationService.NavigateTo(modEditPageVm);
        }

        private async Task Remove()
        {
            await _dllManagerService.RemoveDllAsync(Dll);
        }
    }
}
