using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ModEngine2ConfigTool.Helpers;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using ModEngine2ConfigTool.Views.Controls;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class ProfileEditPageVm : ObservableObject
    {
        private string _lastOpenedLocation;
        private readonly ProfileManagerService _profileManagerService;

        public ProfileVm Profile { get; }

        public string Header { get; }

        public ICommand SelectImageCommand { get; }

        public ICommand SortModsByNameCommand { get; }

        public ICommand SortModsByPathCommand { get; }

        public ICommand SortModsByDescriptionCommand { get; }

        public ICommand SortModsByDateAddedCommand { get; }

        public ICommand RemoveModFromProfileCommand { get; }

        public ICollectionView Mods { get; }

        public ICollectionView ExternalDlls { get; }

        public ProfileEditPageVm(
            ProfileVm profile, 
            bool IsCreatingNewProfile,
            ProfileManagerService profileManagerService)
        {
            _profileManagerService = profileManagerService;

            Profile = profile;
            Header = IsCreatingNewProfile
                ? "Create new Profile"
                : "Edit Profile";

            SelectImageCommand = new RelayCommand(SelectImage);

            _lastOpenedLocation = string.Empty;

            SortModsByNameCommand = new AsyncRelayCommand<SortButtonMode>(SortModsByName);
            SortModsByPathCommand = new AsyncRelayCommand<SortButtonMode>(SortModsByPath);
            SortModsByDescriptionCommand = new AsyncRelayCommand<SortButtonMode>(SortModsByDescription);
            SortModsByDateAddedCommand = new AsyncRelayCommand<SortButtonMode>(SortModsByDateAdded);

            RemoveModFromProfileCommand = new AsyncRelayCommand<ProfileVmModVmTuple>(
                RemoveModFromProfile);

            Mods = CollectionViewSource.GetDefaultView(Profile.Mods);
            ExternalDlls = CollectionViewSource.GetDefaultView(Profile.ExternalDlls);
        }

        private void SelectImage()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "All Picture Files(*.bmp;*.jpg;*.gif;*.ico;*.png;*.wdp;*.tiff)|" +
                    "*.BMP;*.JPG;*.GIF;*.ICO;*.PNG;*.WDP;*.TIFF|" +
                    "All files (*.*)|*.*",
                Multiselect = false,
                Title = "Select Profile Image",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (_lastOpenedLocation.Equals(string.Empty))
            {
                fileDialog.InitialDirectory = !string.IsNullOrWhiteSpace(Profile.ImagePath)
                    ? Path.GetDirectoryName(Profile.ImagePath)
                    : Directory.GetCurrentDirectory();
            }
            else
            {
                fileDialog.InitialDirectory = _lastOpenedLocation;
            }

            if (fileDialog.ShowDialog().Equals(true))
            {
                _lastOpenedLocation = Path.GetDirectoryName(fileDialog.FileName) ?? string.Empty;
                Profile.ImagePath = fileDialog.FileName;
            }
        }

        private async Task RemoveModFromProfile(ProfileVmModVmTuple? tuple)
        {
            if (tuple is null)
            {
                return;
            }

            await _profileManagerService.RemoveModFromProfile(
                tuple.ProfileVm,
                tuple.ModVm);
        }

        private async Task SortModsByName(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    Mods.SortDescriptions.Clear();
                    Mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.Name),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    Mods.SortDescriptions.Clear();
                    Mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.Name),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortModsByDescription(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    Mods.SortDescriptions.Clear();
                    Mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.Description),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    Mods.SortDescriptions.Clear();
                    Mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.Description),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortModsByPath(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    Mods.SortDescriptions.Clear();
                    Mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.FolderPath),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    Mods.SortDescriptions.Clear();
                    Mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.FolderPath),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortModsByDateAdded(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    Mods.SortDescriptions.Clear();
                    Mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.Added),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    Mods.SortDescriptions.Clear();
                    Mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.Added),
                        ListSortDirection.Ascending));
                }
            });
        }
    }
}
