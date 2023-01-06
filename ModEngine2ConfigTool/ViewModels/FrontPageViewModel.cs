using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class FrontPageViewModel : ObservableObject
    {
        private ObservableCollection<ProfileViewModel> profiles;
        private ProfileViewModel? _selectedProfile;

        public string PageName { get; } = nameof(FrontPageViewModel);

        public ObservableCollection<ProfileViewModel> Profiles 
        { 
            get => profiles; 
            set => SetProperty(ref profiles, value); 
        }

        public ProfileViewModel? SelectedProfile 
        {
            get => _selectedProfile;
            set => SetProperty(ref _selectedProfile, value);
        }

        public ICommand AddNewProfileCommand { get; }

        public ICommand CopyProfileCommand { get; }

        public ICommand RenameProfileCommand { get; }

        public ICommand SaveProfileChangesCommand { get; }

        public ICommand RevertProfileChangesCommand { get; }

        public ICommand DeleteProfileCommand { get; }

        public FrontPageViewModel()
        {
            RefreshProfiles();

            SelectedProfile = Profiles.FirstOrDefault();

            AddNewProfileCommand = new AsyncRelayCommand(AddNewProfile);
            SaveProfileChangesCommand = new RelayCommand(SaveProfileChanges);
        }

        private void RefreshProfiles()
        {
            Profiles = new ObservableCollection<ProfileViewModel>(
                ProfileService.LoadProfiles(".\\Profiles"));

            SelectedProfile = Profiles.FirstOrDefault();
        }

        private async Task AddNewProfile()
        {
            var dialogVm = new TextEntryDialogViewModel("New Profile Name", "");

            var dialog = new TextEntryDialog()
            {
                DataContext = dialogVm
            };

            var result = await DialogHost.Show(dialog, App.DialogHostId);

            if (result is not bool || result.Equals(false))
            {
                return;
            }

            var newProfile = new ProfileModel(dialogVm.FieldValue);
            ProfileService.WriteProfile(newProfile);

            RefreshProfiles();
        }

        private void CopyProfile()
        {

        }

        private void RenameProfile()
        {

        }

        private void SaveProfileChanges()
        {
            if(SelectedProfile is null)
            {
                return;
            }

            var profile = new ProfileModel(
                SelectedProfile.Name,
                SelectedProfile.ModFolderListViewModel
                    .ProfileModsList
                    .Select(x => new ModModel(x.Name, x.Location, x.IsEnabled))
                    .ToList(),
                SelectedProfile.DllListViewModel
                    .ProfileModsList
                    .Select(x => new ModModel(x.Name, x.Location, x.IsEnabled))
                    .ToList(),
                SelectedProfile.EnableME2Debug,
                SelectedProfile.EnableModLoaderConfiguration,
                SelectedProfile.EnableScyllaHide);

            ProfileService.WriteProfile(profile);
        }

        private void RevertProfileChanges()
        {

        }

        private void DeleteProfile()
        {

        }
    }
}
