using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class ProfileDllListButtonVm : ObservableObject
    {
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ProfileVm _profileVm;

        public DllVm Dll { get; }

        public ICommand Command { get; }

        public ICommand MoveUpCommand { get; }

        public ICommand MoveDownCommand { get; }

        public ICommand EditCommand { get; }
        
        public ICommand RemoveCommand { get; }

        public ProfileDllListButtonVm(
            ProfileVm profileVm,
            DllVm dllVm,
            NavigationService navigationService,
            ProfileManagerService profileManagerService)
        {
            Dll = dllVm;
            _profileVm = profileVm;
            _profileManagerService = profileManagerService;
            _navigationService = navigationService;

            Command = new AsyncRelayCommand(NavigateToEditModCommand);
            MoveUpCommand = new AsyncRelayCommand(MoveUp);
            MoveDownCommand = new AsyncRelayCommand(MoveDown);
            EditCommand = Command;
            RemoveCommand = new AsyncRelayCommand(Remove);
        }

        private async Task NavigateToEditModCommand()
        {
            await _navigationService.NavigateTo<DllEditPageVm>(
                new NamedParameter("dll", Dll));
        }

        private async Task MoveUp()
        {
            await _profileManagerService.MoveDllInProfile(_profileVm, Dll, -1);
        }

        private async Task MoveDown()
        {
            await _profileManagerService.MoveDllInProfile(_profileVm, Dll, 1);
        }

        private async Task Remove()
        {
            await _profileManagerService.RemoveDllFromProfile(_profileVm, Dll);
        }
    }
}
