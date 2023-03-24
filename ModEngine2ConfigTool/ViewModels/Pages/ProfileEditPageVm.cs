using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using System.Runtime.InteropServices;
using System.Text;
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
        private readonly IconService _iconService;
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
            DialogService dialogService,
            IconService iconService)
        {
            _profileManagerService = profileManagerService;
            _playManagerService = playManagerService;
            _navigationService = navigationService;
            _saveManagerService = saveManagerService;
            _packageService = packageService;
            _dialogService = dialogService;
            _iconService = iconService;

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
                IShellLink link = (IShellLink)new ShellLink();

                // setup shortcut information
                link.SetDescription($"Shortcut to {Profile.Name}.");
                link.SetArguments($"-p \"{Profile.Model.ProfileId}\"");
                link.SetWorkingDirectory(AppDomain.CurrentDomain.BaseDirectory);
                link.SetPath(AppDomain.CurrentDomain.BaseDirectory + "Metis Mod Launcher.exe");

                if (File.Exists(Profile.ImagePath))
                {
                    var iconFile = _iconService.CreateTempIcon(
                        Profile.ImagePath,
                        Profile.Model.ProfileId.ToString());

                    if(iconFile is not null)
                    {
                        link.SetIconLocation(iconFile, 0);
                    }
                }

                // save it
                var file = (System.Runtime.InteropServices.ComTypes.IPersistFile)link;

                file.Save(shortcutFile, false);
            }
        }

        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }
    }
}
