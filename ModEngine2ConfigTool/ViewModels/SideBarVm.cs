using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class SideBarVm : ObservableObject
    {
        public ICommand NavigateHomeCommand { get; }

        public ICommand NavigateProfilesCommand { get; }

        public ICommand NavigateModsCommand { get; }

        public ICommand NavigateExternalDllsCommand { get; }

        public ICommand NavigateSettingsCommand { get; }

        public ICommand NavigateHelpCommand { get; }

        public ICommand NavigateCreateNewProfileCommand { get; }

        private MainPanelVm _mainPanelVm;

        public SideBarVm(MainPanelVm mainPanelVm)
        {
            _mainPanelVm = mainPanelVm;

            NavigateHomeCommand = new AsyncRelayCommand(_mainPanelVm.NavigateHome);
            NavigateProfilesCommand = new AsyncRelayCommand(_mainPanelVm.NavigateProfiles);
            NavigateModsCommand = new AsyncRelayCommand(_mainPanelVm.NavigateMods);
            NavigateExternalDllsCommand = new AsyncRelayCommand(_mainPanelVm.NavigateExternalDlls);
            NavigateSettingsCommand = new AsyncRelayCommand(_mainPanelVm.NavigateSettings);
            NavigateHelpCommand = new AsyncRelayCommand(_mainPanelVm.NavigateHelp);
            NavigateCreateNewProfileCommand = new AsyncRelayCommand(_mainPanelVm.NavigateCreateNewProfile);
        }
    }
}
