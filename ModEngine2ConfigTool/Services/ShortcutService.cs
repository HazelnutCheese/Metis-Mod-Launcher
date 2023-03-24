using ModEngine2ConfigTool.ViewModels.Profiles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ModEngine2ConfigTool.Services
{
    public class ShortcutService
    {
        private readonly IconService _iconService;
        private readonly DialogService _dialogService;
        private readonly string _appDataPath;

        public ShortcutService(
            IconService iconService, 
            DialogService dialogService,
            string cmdAppDataPath = "") 
        {
            _iconService = iconService;
            _dialogService = dialogService;
            _appDataPath = cmdAppDataPath;
        }

        public void CreateShortcut(ProfileVm profile)
        {
            var shortcutFile = _dialogService.ShowSaveFileDialog(
                "Save Shortcut to",
                "All Shortcut Files(*.lnk)|*.lnk",
                "*.lnk",
                defaultFolder: null,
                $"{profile.Name}.lnk");

            if (shortcutFile is not null)
            {
                IShellLink link = (IShellLink)new ShellLink();

                // setup shortcut information
                link.SetDescription($"Shortcut to {profile.Name}.");
                var arguments = $"-p \"{profile.Model.ProfileId}\"";

                if(!string.IsNullOrWhiteSpace(_appDataPath))
                {
                    arguments += $" -d \"{_appDataPath}\"";
                }

                link.SetArguments(arguments);
                link.SetWorkingDirectory(AppDomain.CurrentDomain.BaseDirectory);
                link.SetPath(AppDomain.CurrentDomain.BaseDirectory + "Metis Mod Launcher.exe");

                if (File.Exists(profile.ImagePath))
                {
                    var iconFile = _iconService.CreateTempIcon(
                        profile.ImagePath,
                        profile.Model.ProfileId.ToString());

                    if (iconFile is not null)
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
