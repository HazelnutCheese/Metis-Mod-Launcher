using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services.Interfaces;
using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tommy;

namespace ModEngine2ConfigTool.Services
{
    public class PackageService
    {
        private readonly string _importedMods;
        private readonly string _importedImages;
        private readonly Version _version;
        private readonly DialogService _dialogService;
        private readonly IDatabaseService _databaseService;
        private readonly ModManagerService _modManagerService;

        public PackageService(
            string dataStorage,
            Version version,
            DialogService dialogService,
            IDatabaseService databaseService,
            ModManagerService modManagerService)
        {
            _importedMods = Path.Combine(dataStorage, "Imported", "Mods");
            _importedImages = Path.Combine(dataStorage, "Imported", "Images");

            if (!Directory.Exists(_importedMods))
            {
                Directory.CreateDirectory(_importedMods);
            }

            //if (!Directory.Exists(_importedDlls))
            //{
            //    Directory.CreateDirectory(_importedDlls);
            //}

            if (!Directory.Exists(_importedImages))
            {
                Directory.CreateDirectory(_importedImages);
            }

            _version = version;
            _dialogService = dialogService;
            _databaseService = databaseService;
            //_profileManagerService = profileManagerService;
            _modManagerService = modManagerService;
            //_dllManagerService = dllManagerService;
        }

        //public void ExportProfile(ProfileVm profile, string savePath)
        //{
        //    var profileTempPath = Path.Combine(Path.GetTempPath(), $"{profile.Model.ProfileId}PkgTemp");
        //    if (Directory.Exists(profileTempPath))
        //    {
        //        Directory.Delete(profileTempPath, true);
        //    }

        //    Directory.CreateDirectory(profileTempPath);

        //    if (File.Exists(profile.ImagePath))
        //    {
        //        File.Copy(
        //            profile.ImagePath,
        //            Path.Combine(
        //                profileTempPath,
        //                Path.GetFileName(profile.ImagePath)));
        //    }

        //    CreateProfilePackageDetailsFile(
        //        profile,
        //        Path.Combine(profileTempPath, "ProfileData.toml"));

        //    foreach (var mod in profile.Mods)
        //    {
        //        var modTempPath = Path.Combine(profileTempPath, mod.Model.ModId.ToString());

        //        var modFolderPathInfo = new DirectoryInfo(mod.FolderPath);

        //        var modTempFolderPath = Path.Combine(modTempPath, modFolderPathInfo.Name);

        //        CopyDirectory(modFolderPathInfo.FullName, modTempFolderPath, true);

        //        var modPakageInfoFile = Path.Combine(modTempPath, "ModData.toml");
        //        CreateModPackageDetailsFile(mod, modPakageInfoFile);

        //        if(File.Exists(mod.ImagePath))
        //        {
        //            File.Copy(mod.ImagePath, Path.Combine(modTempPath, Path.GetFileName(mod.ImagePath)));
        //        }
        //    }

        //    foreach (var dll in profile.ExternalDlls)
        //    {
        //        var dllTempPath = Path.Combine(profileTempPath, dll.Model.DllId.ToString());

        //        Directory.CreateDirectory(dllTempPath);

        //        var dllFileTempPath = Path.Combine(dllTempPath, Path.GetFileName(dll.FilePath));

        //        File.Copy(dll.FilePath, dllFileTempPath);

        //        var dllPakageInfoFile = Path.Combine(dllTempPath, "DllData.toml");
        //        CreateDllPackageDetailsFile(dll, dllPakageInfoFile);

        //        if (File.Exists(dll.ImagePath))
        //        {
        //            File.Copy(dll.ImagePath, Path.Combine(dllTempPath, Path.GetFileName(dll.ImagePath)));
        //        }
        //    }

        //    var tempPath = Path.ChangeExtension(savePath, ".metispropkgtmp");
        //    if (File.Exists(tempPath))
        //    {
        //        File.Delete(tempPath);
        //    }

        //    ZipFile.CreateFromDirectory(profileTempPath, tempPath);
        //    File.Move(tempPath, savePath, true);
        //    Directory.Delete(profileTempPath, true);
        //}

        //public void ExportDll(DllVm dll, string savePath)
        //{
        //    var dllTempPath = Path.Combine(Path.GetTempPath(), dll.Model.DllId.ToString());
        //    Directory.CreateDirectory(dllTempPath);

        //    var dllFilePathInfo = new DirectoryInfo(mod.FolderPath);

        //    var modTempFolderPath = Path.Combine(modTempPath, modFolderPathInfo.Name);

        //    CopyDirectory(modFolderPathInfo.FullName, modTempFolderPath, true);

        //    var modPakageInfoFile = Path.Combine(modTempPath, "ModData.toml");
        //    CreateModPackageDetailsFile(mod, modPakageInfoFile);

        //    if (File.Exists(mod.ImagePath))
        //    {
        //        File.Copy(mod.ImagePath, Path.Combine(modTempPath, Path.GetFileName(mod.ImagePath)));
        //    }

        //    var tempPath = Path.ChangeExtension(savePath, ".metismodpkgtmp");
        //    if (File.Exists(tempPath))
        //    {
        //        File.Delete(tempPath);
        //    }
        //    ZipFile.CreateFromDirectory(modTempPath, tempPath);
        //    File.Move(tempPath, savePath, true);
        //    Directory.Delete(modTempPath, true);
        //}

        //        public async Task<ProfileVm> ImportProfile(string packagePath)
        //        {
        //            var importTask = Task.Run(() =>
        //            {
        //                try
        //                {
        //                    var tempPath = Path.Combine(
        //                        Path.GetTempPath(),
        //                        Guid.NewGuid().ToString());

        //                    Directory.CreateDirectory(tempPath);
        //                    ZipFile.ExtractToDirectory(packagePath, tempPath);

        //                    var profileData = Path.Combine(tempPath, "ProfileData.toml");
        //                    var profile = ReadProfilePackageDetailsFile(profileData);

        //                    if (!string.IsNullOrWhiteSpace(profile.ImagePath))
        //                    {
        //                        var imageLocation = Path.Combine(_importedImages, profile.ProfileId.ToString());
        //                        Directory.CreateDirectory(imageLocation);

        //                        var newPath = Path.Combine(imageLocation, profile.ImagePath);
        //                        File.Copy(Path.Combine(tempPath, profile.ImagePath), newPath);

        //                        profile.ImagePath = newPath;
        //                    }

        //                    profile.Mods = new List<Mod>();
        //                    profile.Dlls = new List<Dll>();

        //                    var directories = Directory.GetDirectories(tempPath);

        //                    foreach (var directory in directories)
        //                    {
        //                        var dllFile = Path.Combine(directory, "DllData.toml");
        //                        var modFile = Path.Combine(directory, "ModData.toml");

        //                        if (File.Exists(dllFile))
        //                        {
        //                            var dll = ReadDllPackageDetailsFile(dllFile);
        //                            var destPath = Path.Combine(_importedDlls, dll.DllId.ToString());
        //                            Directory.Move(directory, destPath);
        //                            dll.FilePath = Directory.GetFiles(destPath, "*.dll").Single();

        //                            if (!string.IsNullOrWhiteSpace(dll.ImagePath))
        //                            {
        //                                dll.ImagePath = Path.Combine(destPath, dll.ImagePath);
        //                            }

        //                            profile.Dlls.Add(dll);
        //                            _databaseService.AddDll(dll);
        //                        }
        //                        else if (File.Exists(modFile))
        //                        {
        //                            var mod = ReadModPackageDetailsFile(modFile);
        //                            var destPath = Path.Combine(_importedMods, mod.ModId.ToString());
        //                            Directory.Move(directory, destPath);
        //                            mod.FolderPath = Directory.GetDirectories(destPath).Single();

        //                            if (!string.IsNullOrWhiteSpace(mod.ImagePath))
        //                            {
        //                                mod.ImagePath = Path.Combine(destPath, mod.ImagePath);
        //                            }

        //                            profile.Mods.Add(mod);
        //                            _databaseService.AddMod(mod);
        //                        }
        //                    }

        //                    _databaseService.AddProfile(profile);
        //                    _databaseService.SaveChanges();

        //                    Directory.Delete(tempPath, true);

        //                    return _profileManagerService.ProfileVms.Single(x => x.Model.ProfileId == profile.ProfileId);
        //                }
        //                catch (Exception e)
        //                {
        //                    _databaseService.DiscardChanges();
        //                    throw;
        //                }
        //            });

        //            var importingDialog = new ProgressDialogView();
        //            var importingDialogVm = new CustomDialogViewModel(
        //                "Importing",
        //                $"Waiting for the import process to finish, this may take a while...",
        //                new List<IFieldViewModel>(),
        //                new List<DialogButtonViewModel>()
        //                {
        //                    //new DialogButtonViewModel(
        //                    //    "Force Exit",
        //                    //    CustomDialogViewModel.GetCloseDialogCommand(true, exportingDialog),
        //                    //    isDefault: true)
        //                });
        //            importingDialog.DataContext = importingDialogVm;

        //#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        //            importTask.ContinueWith(t =>
        //            {
        //                _dispatcherService.InvokeUi(() => DialogHost.GetDialogSession(App.DialogHostId)?.Close(true));
        //            });
        //#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        //            await DialogHost.Show(importingDialog, App.DialogHostId);
        //            //await importTask;
        //            await _modManagerService.RefreshAsync();
        //            await _dllManagerService.RefreshAsync();
        //            return importTask.Result;
        //        }

        public async Task ExportMod(ModVm mod, string savePath)
        {
            var cts = new CancellationTokenSource();
            var exportTask = Task.Run(() =>
            {
                string modTempPath = "";
                string tempFilePath = "";

                try
                {
                    CleanupMods();

                    modTempPath = Path.Combine(Path.GetTempPath(), mod.Model.ModId.ToString());
                    Directory.CreateDirectory(modTempPath);

                    var modFolderPathInfo = new DirectoryInfo(mod.FolderPath);

                    var modTempFolderPath = Path.Combine(modTempPath, modFolderPathInfo.Name);

                    CopyDirectory(modFolderPathInfo.FullName, modTempFolderPath, true);

                    cts.Token.ThrowIfCancellationRequested();

                    var modPakageInfoFile = Path.Combine(modTempPath, "ModData.toml");
                    CreateModPackageDetailsFile(mod, modPakageInfoFile);

                    if (File.Exists(mod.ImagePath))
                    {
                        File.Copy(mod.ImagePath, Path.Combine(modTempPath, Path.GetFileName(mod.ImagePath)));
                    }

                    tempFilePath = Path.ChangeExtension(savePath, ".metismodpkgtmp");
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }

                    cts.Token.ThrowIfCancellationRequested();

                    ZipFile.CreateFromDirectory(modTempPath, tempFilePath);
                    File.Move(tempFilePath, savePath, true);
                }
                catch (Exception e)
                {
                    if (e is not TaskCanceledException && e is not OperationCanceledException)
                    {
                        Log.Instance.Error(e.Message);
                        throw;
                    }
                }
                finally
                {
                    if (Directory.Exists(modTempPath))
                    {
                        Directory.Delete(modTempPath, true);
                    }

                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                }
            });

            var exportingDialogVm = new CustomDialogViewModel(
                "Exporting",
                $"Waiting for the export process to finish, this may take a while...",
                fields: null,
                new List<DialogButtonViewModel>
                {
                    new DialogButtonViewModel(
                        "Cancel",
                        result: false,
                        isDefault: false)
                });

            var dialogResult = await _dialogService.ShowProgressDialog(
                exportingDialogVm,
                exportTask);

            if (dialogResult is bool && dialogResult.Equals(false))
            {
                cts.Cancel();
            }

            await exportTask;
        }

        public async Task<ModVm?> ImportMod(string packagePath)
        {
            var cts = new CancellationTokenSource();

            var importTask = Task.Run(async () =>
            {
                string tempPath = string.Empty;
                string destPath = string.Empty;

                try
                {
                    CleanupMods();

                    tempPath = Path.Combine(
                            Path.GetTempPath(),
                            Guid.NewGuid().ToString());

                    Directory.CreateDirectory(tempPath);
                    ZipFile.ExtractToDirectory(packagePath, tempPath);

                    cts.Token.ThrowIfCancellationRequested();

                    var modFile = Path.Combine(tempPath, "ModData.toml");

                    var mod = ReadModPackageDetailsFile(modFile);
                    destPath = Path.Combine(_importedMods, mod.ModId.ToString());
                    Directory.Move(tempPath, destPath);

                    cts.Token.ThrowIfCancellationRequested();

                    mod.FolderPath = Directory.GetDirectories(destPath).Single();

                    if (!string.IsNullOrWhiteSpace(mod.ImagePath))
                    {
                        mod.ImagePath = Path.Combine(destPath, mod.ImagePath);
                    }

                    _databaseService.AddMod(mod);
                    _databaseService.SaveChanges();

                    await _modManagerService.RefreshAsync();
                    //await _dllManagerService.RefreshAsync();

                    return _modManagerService.ModVms.Single(x => x.Model.ModId == mod.ModId);
                }
                catch (Exception e)
                {
                    if (Directory.Exists(destPath))
                    {
                        Directory.Delete(destPath, true);
                    }

                    _databaseService.DiscardChanges();

                    if(e is not TaskCanceledException && e is not OperationCanceledException)
                    {
                        Log.Instance.Error(e.Message);
                        throw;
                    }

                    return null;
                }
                finally
                {
                    if (Directory.Exists(tempPath))
                    {
                        Directory.Delete(tempPath, true);
                    }
                }
            });

            var importingDialogVm = new CustomDialogViewModel(
                "Importing",
                $"Waiting for the import process to finish, this may take a while...",
                fields: null,
                new List<DialogButtonViewModel>
                {
                    new DialogButtonViewModel(
                        "Cancel", 
                        result: false, 
                        isDefault: false)
                });

            var dialogResult = await _dialogService.ShowProgressDialog(
                importingDialogVm, 
                importTask);

            if(dialogResult is bool && dialogResult.Equals(false))
            {
                cts.Cancel();
            }

            return await importTask;
        }

        //public async Task<DllVm> ImportDll(string packagePath)
        //{
        //    try
        //    {
        //        var tempPath = Path.Combine(
        //                Path.GetTempPath(),
        //                Guid.NewGuid().ToString());

        //        Directory.CreateDirectory(tempPath);
        //        ZipFile.ExtractToDirectory(packagePath, tempPath);

        //        var modFile = Path.Combine(tempPath, "DllData.toml");

        //        var dll = ReadDllPackageDetailsFile(modFile);
        //        var destPath = Path.Combine(_importedMods, dll.DllId.ToString());
        //        Directory.Move(tempPath, destPath);
        //        dll.FilePath = Directory.GetFiles(destPath, "*.dll").Single();

        //        if (!string.IsNullOrWhiteSpace(dll.ImagePath))
        //        {
        //            dll.ImagePath = Path.Combine(destPath, dll.ImagePath);
        //        }

        //        _databaseService.AddDll(dll);
        //        _databaseService.SaveChanges();

        //        //Directory.Delete(tempPath, true);
        //        await _dllManagerService.RefreshAsync();

        //        return _dllManagerService.DllVms.Single(x => x.Model.DllId == dll.DllId);
        //    }
        //    catch (Exception e)
        //    {
        //        _databaseService.DiscardChanges();
        //        throw;
        //    }
        //}

        //private static Profile ReadProfilePackageDetailsFile(string filePath)
        //{
        //    var profile = new Profile()
        //    {
        //        ProfileId = Guid.NewGuid(),
        //        Created = DateTime.Now
        //    };

        //    using StreamReader reader = File.OpenText(filePath);

        //    TomlTable table = TOML.Parse(reader);

        //    var version = table["Package"]["version"].AsString;
        //    if (!version.Value.Equals("1.0.0"))
        //    {
        //        throw new InvalidOperationException(
        //            $"This package requires a newer version of Metis. " +
        //            $"Please install the latest release and try again.");
        //    }

        //    var fileType = table["Package"]["fileType"].AsString;
        //    if (!fileType.Value.Equals("profile"))
        //    {
        //        throw new InvalidOperationException(
        //            $"This package can not be imported as a profile, it is a {fileType} package file.");
        //    }

        //    profile.Name = table["Profile"]["name"].AsString.Value;
        //    profile.Description = table["Profile"]["description"].AsString.Value;
        //    profile.ImagePath = table["Profile"]["imagePath"].AsString.Value;

        //    return profile;
        //}

        private Mod ReadModPackageDetailsFile(string filePath)
        {
            var mod = new Mod()
            {
                ModId = Guid.NewGuid(),
                Added = DateTime.Now
            };

            using StreamReader reader = File.OpenText(filePath);

            TomlTable table = TOML.Parse(reader);

            var version = table["Package"]["version"].AsString;
            if (!Version.TryParse(version, out var versionNum) ||
                versionNum.CompareTo(_version) > 0)
            {
                throw new InvalidOperationException(
                    $"This package requires a newer version ({version}) of Metis Mod Launcher. " +
                    $"Please install the latest release and try again.");
            }

            var fileType = table["Package"]["fileType"].AsString;
            if (!fileType.Value.Equals("mod"))
            {
                throw new InvalidOperationException(
                    $"This package can not be imported as a mod, it is a {fileType} package file.");
            }

            mod.Name = table["Mod"]["name"].AsString.Value;
            mod.Description = table["Mod"]["description"].AsString.Value;
            mod.ImagePath = table["Mod"]["imagePath"].AsString.Value;

            return mod;
        }

        //private static Dll ReadDllPackageDetailsFile(string filePath)
        //{
        //    var dll = new Dll()
        //    {
        //        DllId = Guid.NewGuid(),
        //        Added = DateTime.Now
        //    };

        //    using StreamReader reader = File.OpenText(filePath);

        //    TomlTable table = TOML.Parse(reader);

        //    var version = table["Package"]["version"].AsString;
        //    if (!version.Value.Equals("1.0.0"))
        //    {
        //        throw new InvalidOperationException(
        //            $"This package requires a newer version of Metis. " +
        //            $"Please install the latest release and try again.");
        //    }

        //    var fileType = table["Package"]["fileType"].AsString;
        //    if (!fileType.Value.Equals("dll"))
        //    {
        //        throw new InvalidOperationException(
        //            $"This package can not be imported as a dll, it is a {fileType} package file.");
        //    }

        //    dll.Name = table["Dll"]["name"].AsString.Value;
        //    dll.Description = table["Dll"]["description"].AsString.Value;
        //    dll.ImagePath = table["Dll"]["imagePath"].AsString.Value;

        //    return dll;
        //}

        //private static void CreateProfilePackageDetailsFile(ProfileVm profile, string filePath)
        //{
        //    using TextWriter writer = File.CreateText(filePath);

        //    var dllListToml = new TomlArray();
        //    foreach (var dll in profile.ExternalDlls)
        //    {
        //        dllListToml.Add(dll.Model.DllId.ToString());
        //    }

        //    var modsListToml = new TomlArray();
        //    foreach (var mod in profile.Mods)
        //    {
        //        modsListToml.Add(mod.Model.ModId.ToString());
        //    }

        //    var fileToml = new TomlTable
        //    {
        //        ["Package"] =
        //        {
        //            ["version"] = "1.0.0",
        //            ["fileType"] = "profile"
        //        },
        //        ["Profile"] =
        //        {
        //            ["name"] = profile.Name,
        //            ["description"] = profile.Description,
        //            ["imagePath"] = File.Exists(profile.ImagePath)
        //                ? Path.GetFileName(profile.ImagePath)
        //                : "",
        //            ["mods"] = modsListToml,
        //            ["dlls"] = dllListToml
        //        }
        //    };

        //    fileToml.WriteTo(writer);
        //    writer.Flush();
        //}

        private void CreateModPackageDetailsFile(ModVm mod, string filePath)
        {
            using TextWriter writer = File.CreateText(filePath);

            var fileToml = new TomlTable
            {
                ["Package"] =
                {
                    ["version"] = _version.ToString(),
                    ["fileType"] = "mod"
                },
                ["Mod"] =
                {
                    ["name"] = mod.Name,
                    ["description"] = mod.Description,
                    ["imagePath"] = File.Exists(mod.ImagePath)
                        ? Path.GetFileName(mod.ImagePath)
                        : ""
                }
            };

            fileToml.WriteTo(writer);
            writer.Flush();
        }

        //private static void CreateDllPackageDetailsFile(DllVm dll, string filePath)
        //{
        //    using TextWriter writer = File.CreateText(filePath);

        //    var fileToml = new TomlTable
        //    {
        //        ["Package"] =
        //        {
        //            ["version"] = "1.0.0",
        //            ["fileType"] = "dll"
        //        },
        //        ["Dll"] =
        //        {
        //            ["name"] = dll.Name,
        //            ["description"] = dll.Description,
        //            ["imagePath"] = File.Exists(dll.ImagePath)
        //                ? Path.GetFileName(dll.ImagePath)
        //                : ""
        //        }
        //    };

        //    fileToml.WriteTo(writer);
        //    writer.Flush();
        //}

        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (var subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        private void CleanupMods()
        {
            var modFolders = Directory.GetDirectories(_importedMods);
            var mods = _modManagerService.ModVms;

            var toDelete = modFolders.Where(
                x => !mods.Any(
                    y => y.Model.ModId.ToString().Equals(new DirectoryInfo(x).Name) || y.FolderPath.Equals(x)));

            foreach(var directory in toDelete)
            {
                Directory.Delete(directory, true);
            }
        }
    }
}
