using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Controls
{
    public class ProfileModListButtonVm : ObservableObject
    {
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;
        private readonly ProfileVm _profileVm;

        public ModVm Mod { get; }

        public ICommand Command { get; }

        public ICommand MoveUpCommand { get; }

        public ICommand MoveDownCommand { get; }

        public ICommand EditCommand { get; }
        
        public ICommand RemoveCommand { get; }

        public ProfileModListButtonVm(
            ProfileVm profileVm,
            ModVm modVm,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService)
        {
            Mod = modVm;
            _profileVm = profileVm;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            _navigationService = navigationService;

            Command = new AsyncRelayCommand(NavigateToEditModCommand);
            MoveUpCommand = new AsyncRelayCommand(MoveUp);
            MoveDownCommand = new AsyncRelayCommand(MoveDown);
            EditCommand = Command;
            RemoveCommand = new AsyncRelayCommand(Remove);
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

        private async Task MoveUp()
        {
            await _profileManagerService.MoveModInProfile(_profileVm, Mod, -1);
        }

        private async Task MoveDown()
        {
            await _profileManagerService.MoveModInProfile(_profileVm, Mod, 1);
        }

        private async Task Remove()
        {
            await _profileManagerService.RemoveModFromProfile(_profileVm, Mod);
        }
    }
}
