using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Extensions;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.ViewModels.Fields;
using ModEngine2ConfigTool.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class FrontPageViewModel : ObservableObject
    {
        private enum LaunchMode
        {
            WithMods,
            WithoutMods
        }

        private ObservableCollection<ProfileViewModel> _profiles;
        private ProfileViewModel? _selectedProfile;
        private LaunchMode _selectedLaunchMode;
        private bool _gameIsRunning;

        public string PageName { get; } = nameof(FrontPageViewModel);

        public ObservableCollection<ProfileViewModel> Profiles
        {
            get => _profiles;
            set => SetProperty(ref _profiles, value);
        }

        public bool GameIsRunning
        {
            get => _gameIsRunning;
            set
            {
                SetProperty(ref _gameIsRunning, value);
                LaunchCommand.NotifyCanExecuteChanged();
            }
        }

        public ProfileViewModel? SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (_selectedProfile is not null)
                {
                    _selectedProfile.PropertyChanged -= SelectedProfile_PropertyChanged;
                }

                SetProperty(ref _selectedProfile, value);

                CopyProfileCommand.NotifyCanExecuteChanged();
                RenameProfileCommand.NotifyCanExecuteChanged();
                SaveProfileChangesCommand.NotifyCanExecuteChanged();
                RevertProfileChangesCommand.NotifyCanExecuteChanged();
                DeleteProfileCommand.NotifyCanExecuteChanged();
                SelectLaunchEldenRingWithProfileCommand.NotifyCanExecuteChanged();
                LaunchCommand.NotifyCanExecuteChanged();
                OnPropertyChanged(nameof(LaunchButtonContent));

                if (_selectedProfile is not null)
                {
                    _selectedProfile.PropertyChanged += SelectedProfile_PropertyChanged;
                }
            }
        }

        public string LaunchButtonContent
        {
            get
            {
                if(Equals(_selectedLaunchMode, LaunchMode.WithMods))
                {
                    return $"Launch Elden Ring ({SelectedProfile?.Name ?? "No Profile Selected"})";
                }
                else
                {
                    return "Launch Elden Ring (No Mods)";
                }
            }
        }

        private LaunchMode SelectedLaunchMode
        {
            get => _selectedLaunchMode;
            set
            {
                _selectedLaunchMode = value;
                OnPropertyChanged(nameof(LaunchButtonContent));
                LaunchCommand.NotifyCanExecuteChanged();
            }
        }

        public ICommand AddNewProfileCommand { get; }

        public ICommand CopyProfileCommand { get; }

        public ICommand RenameProfileCommand { get; }

        public ICommand SaveProfileChangesCommand { get; }

        public ICommand RevertProfileChangesCommand { get; }

        public ICommand DeleteProfileCommand { get; }

        public ICommand LaunchCommand { get; }

        public ICommand SelectLaunchEldenRingCommand { get; }

        public ICommand SelectLaunchEldenRingWithProfileCommand { get; }

        public FrontPageViewModel()
        {
            _profiles = new ObservableCollection<ProfileViewModel>(
                ProfileService.LoadProfiles());

            SelectedProfile = Profiles.FirstOrDefault();

            if(SelectedProfile is null)
            {
                SelectLaunchEldenRing();
            }
            else
            {
                SelectLaunchEldenRingWithProfile();
            }

            AddNewProfileCommand = new AsyncRelayCommand(AddNewProfile);

            CopyProfileCommand = new RelayCommand(
                CopyProfile,
                () => SelectedProfile is not null);

            RenameProfileCommand = new AsyncRelayCommand(
                RenameProfile,
                () => SelectedProfile is not null);

            SaveProfileChangesCommand = new AsyncRelayCommand(
                SaveProfileChanges,
                () => SelectedProfile?.IsChanged ?? false);

            RevertProfileChangesCommand = new AsyncRelayCommand(
                RevertProfileChanges,
                () => SelectedProfile?.IsChanged ?? false);

            DeleteProfileCommand = new AsyncRelayCommand(
                DeleteProfile,
                () => SelectedProfile is not null);

            LaunchCommand = new AsyncRelayCommand(
                Launch,
                () => !GameIsRunning && (Equals(_selectedLaunchMode, LaunchMode.WithoutMods) 
                    || (SelectedProfile is not null && !SelectedProfile.IsChanged)));

            SelectLaunchEldenRingCommand = new RelayCommand(SelectLaunchEldenRing);

            SelectLaunchEldenRingWithProfileCommand = new RelayCommand(
                SelectLaunchEldenRingWithProfile,
                () => SelectedProfile is not null && !SelectedProfile.IsChanged);
        }

        private async Task AddNewProfile()
        {
            var dialog = new CustomDialogView();

            var nameField = new TextFieldViewModel(
                "New Name:",
                "Enter the name for the new profile",
                string.Empty,
                new List<ValidationRule<string>>
                {
                    CommonValidationRules.NotEmpty("The profile must have a name."),
                    CommonValidationRules.IsValidFilename("{0} is not a valid profile name.", ".\\\\Profiles\\\\{0}.toml"),
                    new ValidationRule<string>(s => CheckProfileDoesNotExist(s), "{0} is an existing profile name.")
                });

            var dialogVm = new CustomDialogViewModel(
                "Add new profile",
                "Choose the name for the new profile." +
                "\n\nNote: The name cannot be the same as any existing profiles name.",
                new List<IFieldViewModel>()
                {
                    nameField
                },
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel(
                        "Accept",
                        CustomDialogViewModel.GetCloseDialogCommand(true, dialog, () => !nameField.HasErrors),
                        isDefault: false),
                    new DialogButtonViewModel(
                        "Cancel",
                        CustomDialogViewModel.GetCloseDialogCommand(false, dialog),
                        isDefault: true)
                });

            dialog.DataContext = dialogVm;

            var result = await DialogHost.Show(dialog, App.DialogHostId);

            if (result is not bool || result.Equals(false))
            {
                return;
            }

            var newProfileName = ((TextFieldViewModel)dialogVm.Fields.Fields.Single()).Value;

            var newProfile = new ProfileModel(newProfileName);
            ProfileService.WriteProfile(newProfile);

            var newProfileVm = new ProfileViewModel(newProfile);
            Profiles.Add(newProfileVm);

            SelectedProfile = newProfileVm;
        }

        private void CopyProfile()
        {
            if (SelectedProfile is null)
            {
                return;
            }

            var newProfileName = SelectedProfile.Name;
            while(Profiles.Any(
                x => string.Equals(x.Name, newProfileName, StringComparison.OrdinalIgnoreCase)))
            {
                newProfileName = $"{newProfileName} - Copy";
            };

            var profile = new ProfileModel(
                newProfileName,
                SelectedProfile.ModFolderListViewModel
                    .OnDiskObjectList
                    .Select(x => new ModModel(x.Name, x.Location, x.IsEnabled))
                    .ToList(),
                SelectedProfile.DllListViewModel
                    .OnDiskObjectList
                    .Select(x => new ExternalDllModel(x.Location))
                    .ToList(),
                ((BoolFieldViewModel)SelectedProfile.Fields.Fields[0]).Value,
                ((BoolFieldViewModel)SelectedProfile.Fields.Fields[1]).Value,
                ((BoolFieldViewModel)SelectedProfile.Fields.Fields[2]).Value);

            ProfileService.WriteProfile(profile);
            SaveManagerService.CopySaves(SelectedProfile.OriginalName, newProfileName);

            var savedProfile = ProfileService.ReadProfile(profile.Name);
            var savedProfileVm = new ProfileViewModel(savedProfile);

            Profiles.Add(savedProfileVm);
            SelectedProfile = savedProfileVm;
        }

        private async Task RenameProfile()
        {
            if (SelectedProfile is null)
            {
                return;
            }

            var dialog = new CustomDialogView();

            var newNameField = new TextFieldViewModel(
                "New Name:",
                "Enter the new name for the profile",
                SelectedProfile.Name,
                new List<ValidationRule<string>>
                {
                    CommonValidationRules.NotEmpty("The profile must have a name."),
                    CommonValidationRules.IsValidFilename("{0} is not a valid profile name.", ".\\\\Profiles\\\\{0}.toml"),
                    new ValidationRule<string>(
                        s => CheckProfileDoesNotExist(
                            s,
                            SelectedProfile.Name),
                        "{0} is an existing profile name.")
                });

            var dialogVm = new CustomDialogViewModel(
                "Rename profile",
                "Choose the new name for the profile." +
                "\n\nNote: The name cannot be the same as any existing profiles name.",
                new List<IFieldViewModel>()
                {
                    newNameField
                },
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel(
                        "Accept",
                        CustomDialogViewModel.GetCloseDialogCommand(true, dialog, () => !newNameField.HasErrors),
                        isDefault: false),
                    new DialogButtonViewModel(
                        "Cancel",
                        CustomDialogViewModel.GetCloseDialogCommand(false, dialog),
                        isDefault: true)
                });

            dialog.DataContext = dialogVm;

            var result = await DialogHost.Show(dialog, App.DialogHostId);

            if (result is not bool || result.Equals(false))
            {
                return;
            }

            var newProfileName = ((TextFieldViewModel)dialogVm.Fields.Fields.Single()).Value;

            SelectedProfile.Name = newProfileName;
        }

        private async Task SaveProfileChanges()
        {
            if (SelectedProfile is null)
            {
                return;
            }

            var dialog = new CustomDialogView();
            var dialogVm = new CustomDialogViewModel(
                "Save Changes",
                "Are you sure you want to save these changes?",
                new List<IFieldViewModel>(),
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel("Save", CustomDialogViewModel.GetCloseDialogCommand(true, dialog), false),
                    new DialogButtonViewModel("Cancel", CustomDialogViewModel.GetCloseDialogCommand(false, dialog), true)
                });

            dialog.DataContext = dialogVm;

            var result = await DialogHost.Show(dialog, App.DialogHostId);

            if (result is not bool || result.Equals(false))
            {
                return;
            }

            var oldName = SelectedProfile.OriginalName;

            var profile = new ProfileModel(
                SelectedProfile.Name,
                SelectedProfile.ModFolderListViewModel
                    .OnDiskObjectList
                    .Select(x => new ModModel(x.Name, x.Location, x.IsEnabled))
                    .ToList(),
                SelectedProfile.DllListViewModel
                    .OnDiskObjectList
                    .Select(x => new ExternalDllModel(x.Location))
                    .ToList(),
                ((BoolFieldViewModel)SelectedProfile.Fields.Fields[0]).Value,
                ((BoolFieldViewModel)SelectedProfile.Fields.Fields[1]).Value,
                ((BoolFieldViewModel)SelectedProfile.Fields.Fields[2]).Value);

            ProfileService.WriteProfile(profile);
            var savedProfile = ProfileService.ReadProfile(SelectedProfile.Name);
            var savedProfileVm = new ProfileViewModel(savedProfile);

            if(SelectedProfile.NameIsChanged)
            {
                SaveManagerService.CopySaves(SelectedProfile.OriginalName, SelectedProfile.Name);
                SaveManagerService.DeleteSaves(SelectedProfile.OriginalName);
                ProfileService.DeleteProfile(SelectedProfile.OriginalName);
            }

            Profiles.Remove(SelectedProfile);

            Profiles.Add(savedProfileVm);
            SelectedProfile = savedProfileVm;
        }

        private async Task RevertProfileChanges()
        {
            if (SelectedProfile is null)
            {
                return;
            }

            var dialog = new CustomDialogView();
            var dialogVm = new CustomDialogViewModel(
                "Revert Changes",
                "Are you sure you want to revert these changes?\n\nNote: Once reverted changes cannot be recovered.",
                new List<IFieldViewModel>(),
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel("Revert", CustomDialogViewModel.GetCloseDialogCommand(true, dialog), false),
                    new DialogButtonViewModel("Cancel", CustomDialogViewModel.GetCloseDialogCommand(false, dialog), true)
                });

            dialog.DataContext = dialogVm;

            var result = await DialogHost.Show(dialog, App.DialogHostId);

            if (result is not bool || result.Equals(false))
            {
                return;
            }

            var revertedProfile = new ProfileViewModel(
                ProfileService.ReadProfile(SelectedProfile.OriginalName));

            Profiles.Remove(SelectedProfile);
            Profiles.Add(revertedProfile);
            SelectedProfile = revertedProfile;
        }

        private async Task DeleteProfile()
        {
            if (SelectedProfile is null)
            {
                return;
            }

            var dialog = new CustomDialogView();
            var dialogVm = new CustomDialogViewModel(
                $"Delete {SelectedProfile.Name}",
                $"Are you sure you want to delete {SelectedProfile.Name} and all associated saves and backups?\n\nNote: Once deleted a profile and it's saves cannot be recovered.",
                new List<IFieldViewModel>(),
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel("Delete", CustomDialogViewModel.GetCloseDialogCommand(true, dialog), false),
                    new DialogButtonViewModel("Cancel", CustomDialogViewModel.GetCloseDialogCommand(false, dialog), true)
                });

            dialog.DataContext = dialogVm;

            var result = await DialogHost.Show(dialog, App.DialogHostId);

            if (result is not bool || result.Equals(false))
            {
                return;
            }

            ProfileService.DeleteProfile(SelectedProfile.Name);
            SaveManagerService.DeleteSaves(SelectedProfile.Name);

            Profiles.Remove(SelectedProfile);
            SelectedProfile = Profiles.FirstOrDefault();
        }

        public async Task Launch()
        {
            if(GameIsRunning)
            {
                return;
            }

            if(Equals(_selectedLaunchMode, LaunchMode.WithMods))
            {
                await LaunchWithMods();
            }
            else
            {
                await LaunchWithoutMods();
            }
        }

        private async Task LaunchWithMods()
        {
            if (SelectedProfile is null || SelectedProfile.IsChanged)
            {
                return;
            }

            GameIsRunning = true;

            SaveManagerService.CreateUnmoddedBackups();
            SaveManagerService.CreateBackups(SelectedProfile.Name);
            SaveManagerService.InstallProfileSaves(SelectedProfile.Name);

            using var modEngineProcess = ModEngine2Service.LaunchWithProfile(SelectedProfile.Name);

            await modEngineProcess.WaitForExitAsync();
            await ModEngine2Service.WaitForEldenRingExit();

            SaveManagerService.UninstallProfileSaves(SelectedProfile.Name);

            GameIsRunning = false;
        }

        private async Task LaunchWithoutMods()
        {
            GameIsRunning = true;

            SaveManagerService.CreateUnmoddedBackups();
            ModEngine2Service.LaunchWithoutMods();
            await ModEngine2Service.WaitForEldenRingExit();

            GameIsRunning = false;
        }

        private void SelectLaunchEldenRing()
        {            
            SelectedLaunchMode = LaunchMode.WithoutMods;
        }

        private void SelectLaunchEldenRingWithProfile()
        {
            if(SelectedProfile is null)
            {
                return;
            }

            SelectedLaunchMode = LaunchMode.WithMods;
        }

        private static bool CheckNewProfileNameValid(string newProfileName)
        {
            var fileName = $".\\Profiles\\{newProfileName}.toml";

            try
            {
                File.OpenRead(fileName).Close();
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (Exception)
            {
            }

            return true;
        }

        private bool CheckProfileDoesNotExist(string newProfileName, string? excludeName = null)
        {
            return !Profiles
                .Where(x => !string.Equals(x.Name, excludeName, StringComparison.OrdinalIgnoreCase))
                .Any(x => 
                    string.Equals(x.Name, newProfileName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.OriginalName, newProfileName, StringComparison.OrdinalIgnoreCase));
        }

        private void SelectedProfile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (Equals(e.PropertyName, nameof(IIsChanged.IsChanged)))
            {
                SaveProfileChangesCommand.NotifyCanExecuteChanged();
                RevertProfileChangesCommand.NotifyCanExecuteChanged();
                LaunchCommand.NotifyCanExecuteChanged();
                SelectLaunchEldenRingWithProfileCommand.NotifyCanExecuteChanged();
            }
        }
    }
}
