using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IWshRuntimeLibrary;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class ProfileEditPageVm : ObservableObject
    {
        private string _lastOpenedLocation;
        private readonly NavigationService _navigationService;
        private readonly SaveManagerService _saveManagerService;
        private readonly PackageService _packageService;
        private readonly DialogService _dialogService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly PlayManagerService _playManagerService;

        public ProfileVm Profile { get; }

        public ICommand SelectImageCommand { get; }

        public ObservableCollection<ProfileModListButtonVm> Mods { get; }

        public ObservableCollection<ProfileDllListButtonVm> Dlls { get; }

        public HotBarVm HotBarVm { get; }

        public HotBarVm SavesHotBarVm { get; }

        public HotBarMenuButtonVm AddMods { get; }

        public HotBarMenuButtonVm AddDlls { get; }

        public bool CanSelectMods { get; }

        public bool CanSelectDlls { get; }

        public string SelectModsText { get; }

        public string SelectDllsText { get; }

        public ProfileEditPageVm(
            ProfileVm profile,
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService,
            DllManagerService dllManagerService,
            PlayManagerService playManagerService,
            SaveManagerService saveManagerService,
            PackageService packageService,
            DialogService dialogService)
        {
            _profileManagerService = profileManagerService;
            _playManagerService = playManagerService;
            _navigationService = navigationService;
            _saveManagerService = saveManagerService;
            _packageService = packageService;
            _dialogService = dialogService;

            Profile = profile;

            SelectImageCommand = new RelayCommand(SelectImage);

            _lastOpenedLocation = string.Empty;

            HotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    new HotBarButtonVm(
                        "Play Profile",
                        PackIconKind.PlayBoxOutline,
                        async () => await PlayAsync()),
                    new HotBarButtonVm(
                        "Copy Profile",
                        PackIconKind.ContentDuplicate,
                        async () => await DuplicateProfileAsync()),
                    new HotBarButtonVm(
                        "Delete Profile",
                        PackIconKind.DeleteOutline,
                        async () => await DeleteProfileAsync()),
                    new HotBarButtonVm(
                        "Create Shortcut",
                        PackIconKind.LinkPlus,
                        () => CreateShortcut()),
                    //new HotBarButtonVm(
                    //    "Export Package",
                    //    PackIconKind.PackageVariantClosed,
                    //    () => ExportPackage()),
                    });

            AddMods = new HotBarMenuButtonVm(
                "Add Mods",
                PackIconKind.FileExportOutline,
                modManagerService.ModVms
                    .Select(x => new HotBarMenuButtonItemVm(
                        x.Name,
                        async () => await profileManagerService.AddModToProfile(profile, x)))
                    .ToList());

            CanSelectMods = modManagerService.ModVms.Any();
            SelectModsText = CanSelectMods
                ? "Select Mods"
                : "No mods avaliable";

            AddDlls = new HotBarMenuButtonVm(
                "Add Dlls",
                PackIconKind.FolderMultiplePlusOutline,
                dllManagerService.DllVms
                    .Select(x => new HotBarMenuButtonItemVm(
                        x.Name,
                        async () => await profileManagerService.AddDllToProfile(profile, x)))
                    .ToList());

            CanSelectDlls = dllManagerService.DllVms.Any();
            SelectDllsText = CanSelectDlls
                ? "Select External Dlls"
                : "No External Dlls avaliable";

            SavesHotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    new HotBarButtonVm(
                        "Go To Saves",
                        PackIconKind.ContentSaveOutline,
                        OpenSavesFolder,
                        HasSaves),
                    new HotBarButtonVm(
                        "Import Saves",
                        PackIconKind.ContentSavePlusOutline,
                        ImportSaves),
                    new HotBarButtonVm(
                        "Backup Saves",
                        PackIconKind.ContentSaveMoveOutline,
                        BackupSaves,
                        HasSaves),
                    new HotBarButtonVm(
                        "Delete Saves",
                        PackIconKind.ContentSaveMinusOutline,
                        DeleteSaves,
                        HasSaves)
                    });

            Mods = new ObservableCollection<ProfileModListButtonVm>(
                Profile.Mods.Select(x => new ProfileModListButtonVm(
                    profile,
                    x,
                    navigationService,
                    profileManagerService)));

            Dlls = new ObservableCollection<ProfileDllListButtonVm>(
                Profile.ExternalDlls.Select(x => new ProfileDllListButtonVm(
                    profile,
                    x,
                    navigationService,
                    profileManagerService)));

            profile.Mods.CollectionChanged += ProfileMods_CollectionChanged;
            profile.ExternalDlls.CollectionChanged += ProfileDlls_CollectionChanged;
        }

        private void ProfileMods_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Mods.Clear();
            foreach (var mod in Profile.Mods)
            {
                Mods.Add(new ProfileModListButtonVm(
                    Profile,
                    mod,
                    _navigationService,
                    _profileManagerService));
            }
        }

        private void ProfileDlls_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            Dlls.Clear();
            foreach (var dll in Profile.ExternalDlls)
            {
                Dlls.Add(new ProfileDllListButtonVm(
                    Profile,
                    dll,
                    _navigationService,
                    _profileManagerService));
            }
        }

        private void SelectImage()
        {
            var imageFile = _dialogService.ShowOpenFileDialog(
                "Select Profile Image",
                DialogService.AllImageFilter,
                Profile.ImagePath,
                Profile.ImagePath);

            if (System.IO.File.Exists(imageFile))
            {
                Profile.ImagePath = imageFile;
            }
        }

        private async Task PlayAsync()
        {
            await _playManagerService.Play(Profile);
            (SavesHotBarVm.Buttons[0] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
            (SavesHotBarVm.Buttons[2] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
            (SavesHotBarVm.Buttons[3] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
        }

        private async Task DuplicateProfileAsync()
        {
            var newProfile = await _profileManagerService.DuplicateProfileAsync(Profile);

            await _navigationService.NavigateTo<ProfileEditPageVm>(
                new NamedParameter("profile", newProfile));
        }

        private async Task DeleteProfileAsync()
        {
            await _profileManagerService.RemoveProfileAsync(Profile);
            await _navigationService.NavigateTo<ProfilesPageVm>();
        }

        private void OpenSavesFolder()
        {
            _saveManagerService.OpenProfileSavesFolder(Profile.Model.ProfileId.ToString());
        }

        private void BackupSaves()
        {
            _saveManagerService.BackupSaves(Profile.Model.ProfileId.ToString());
        }

        private void ImportSaves()
        {
            _saveManagerService.ImportSave(Profile.Model.ProfileId.ToString());
            (SavesHotBarVm.Buttons[0] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
            (SavesHotBarVm.Buttons[2] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
            (SavesHotBarVm.Buttons[3] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
        }

        private void DeleteSaves()
        {
            _saveManagerService.DeleteSaves(Profile.Model.ProfileId.ToString());
            (SavesHotBarVm.Buttons[0] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
            (SavesHotBarVm.Buttons[2] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
            (SavesHotBarVm.Buttons[3] as HotBarButtonVm)?.RaiseNotifyCommandExecuteChanged();
        }

        private bool HasSaves()
        {
            return _saveManagerService.ProfileHasSaves(Profile.Model.ProfileId.ToString());
        }

        private void CreateShortcut()
        {
            var shortcutFile = _dialogService.ShowSaveFileDialog(
                "Save Shortcut to",
                "All Shortcut Files(*.lnk)|*.lnk",
                "*.lnk",
                defaultFolder: null,
                $"{Profile.Name}.lnk");

            if (shortcutFile is not null)
            {
                var shell = new WshShell();
                IWshShortcut shortcut = shell.CreateShortcut(shortcutFile);
                shortcut.TargetPath = AppDomain.CurrentDomain.BaseDirectory + "Metis Mod Launcher.exe";
                shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                shortcut.Arguments = $"-p \"{Profile.Model.ProfileId}\"";

                if (System.IO.File.Exists(Profile.ImagePath))
                {
                    var iconPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        $"Metis Mod Launcher\\Temp\\{Profile.Model.ProfileId}.ico");

                    var image = Image.FromFile(Profile.ImagePath);
                    var icon = IconFromImage(image);
                    using var filestream = new FileStream(
                        iconPath,
                        FileMode.Create);
                    icon.Save(filestream);
                    icon.Dispose();
                    image.Dispose();

                    shortcut.IconLocation = iconPath;
                }

                shortcut.Save();
            }
        }

        //private void ExportPackage()
        //{
        //    const string fileExtension = "metispropkg";

        //    var fileDialog = new SaveFileDialog
        //    {
        //        Filter = $"Profile package Files(*.{fileExtension})|*.{fileExtension}",
        //        Title = "Save Package File",
        //        AddExtension = true,
        //        FileName = $"{Profile.Name}.{fileExtension}"
        //    };

        //    if (fileDialog.ShowDialog().Equals(true))
        //    {
        //        _packageService.ExportProfile(Profile, fileDialog.FileName);
        //    }
        //}

        private static Icon IconFromImage(Image img)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            // Header
            bw.Write((short)0);   // 0 : reserved
            bw.Write((short)1);   // 2 : 1=ico, 2=cur
            bw.Write((short)1);   // 4 : number of images
                                  // Image directory
            var w = img.Width;
            if (w >= 256)
            {
                w = 0;
            }

            bw.Write((byte)w);    // 0 : width of image
            var h = img.Height;
            if (h >= 256)
            {
                h = 0;
            }

            bw.Write((byte)h);    // 1 : height of image
            bw.Write((byte)0);    // 2 : number of colors in palette
            bw.Write((byte)0);    // 3 : reserved
            bw.Write((short)0);   // 4 : number of color planes
            bw.Write((short)0);   // 6 : bits per pixel
            var sizeHere = ms.Position;
            bw.Write((int)0);     // 8 : image size
            var start = (int)ms.Position + 4;
            bw.Write(start);      // 12: offset of image data
                                  // Image data
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var imageSize = (int)ms.Position - start;
            ms.Seek(sizeHere, System.IO.SeekOrigin.Begin);
            bw.Write(imageSize);
            ms.Seek(0, System.IO.SeekOrigin.Begin);

            // And load it
            return new Icon(ms);
        }
    }
}
