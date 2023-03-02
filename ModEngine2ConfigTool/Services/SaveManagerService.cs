using ModEngine2ConfigTool.Services.Interfaces;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace ModEngine2ConfigTool.Services
{
    public class SaveManagerService : ISaveManagerService
    {
        private const string _savesFolderName = "Saves";
        private const string _backupsFolderName = "Backups";
        private const string _baseGameBackupsFolderName = "Vanilla";

        private readonly string _savesRoot;

        private readonly string _backupsRoot;
        private readonly IFileSystem _fileSystem;
        private readonly IDialogService _dialogService;

        public SaveManagerService(
            string dataStorage,
            IFileSystem fileSystem,
            IDialogService dialogService)
        {
            _savesRoot = Path.Combine(dataStorage, _savesFolderName);
            _backupsRoot = Path.Combine(_savesRoot, _backupsFolderName);

            _fileSystem = fileSystem;
            _dialogService = dialogService;

            if (!_fileSystem.Directory.Exists(_savesRoot))
            {
                _fileSystem.Directory.CreateDirectory(_savesRoot);
            }

            if (!_fileSystem.Directory.Exists(_backupsRoot))
            {
                _fileSystem.Directory.CreateDirectory(_backupsRoot);
            }
        }

        public void InstallProfileSaves(string profileName)
        {
            var eldenRingSaveGameFolder = GetEldenRingSavesFolder();
            if (eldenRingSaveGameFolder is null)
            {
                return;
            }

            var profileSaves = Path.Combine(
                _savesRoot,
                profileName);

            if (_fileSystem.Directory.Exists(eldenRingSaveGameFolder))
            {
                var existingSaveGames = GetSaves(eldenRingSaveGameFolder, "sl2");
                var existingCoopSaveGames = GetSaves(eldenRingSaveGameFolder, "co2");

                foreach (var file in existingSaveGames)
                {
                    var newExtension = $"{Path.GetExtension(file)}temp";
                    _fileSystem.File.Move(file, Path.ChangeExtension(file, newExtension), true);
                }

                foreach (var file in existingCoopSaveGames)
                {
                    var newExtension = $"{Path.GetExtension(file)}temp";
                    _fileSystem.File.Move(file, Path.ChangeExtension(file, newExtension), true);
                }
            }

            if (_fileSystem.Directory.Exists(profileSaves))
            {
                var existingSaveGames = GetSaves(profileSaves, "sl2");
                var existingCoopSaveGames = GetSaves(profileSaves, "co2");

                foreach (var file in existingSaveGames)
                {
                    var fileName = Path.GetFileName(file);
                    _fileSystem.File.Copy(file, $"{eldenRingSaveGameFolder}\\{fileName}");
                }

                foreach (var file in existingCoopSaveGames)
                {
                    var fileName = Path.GetFileName(file);
                    _fileSystem.File.Copy(file, $"{eldenRingSaveGameFolder}\\{fileName}");
                }
            }
        }

        public void UninstallProfileSaves(string profileName)
        {
            var eldenRingSaveGameFolder = GetEldenRingSavesFolder();
            if (eldenRingSaveGameFolder is null)
            {
                return;
            }

            var profileSaves = Path.Combine(
                _savesRoot,
                profileName);

            _fileSystem.Directory.CreateDirectory(profileSaves);

            if (_fileSystem.Directory.Exists(eldenRingSaveGameFolder))
            {
                var existingSaveGames = GetSaves(eldenRingSaveGameFolder, "sl2");
                var existingCoopSaveGames = GetSaves(eldenRingSaveGameFolder, "co2");

                foreach (var file in existingSaveGames)
                {
                    var fileName = Path.GetFileName(file);
                    _fileSystem.File.Move(file, $"{profileSaves}\\{fileName}", true);
                }

                foreach (var file in existingCoopSaveGames)
                {
                    var fileName = Path.GetFileName(file);
                    _fileSystem.File.Move(file, $"{profileSaves}\\{fileName}", true);
                }

                var tempSaveGames = GetTempSaves(eldenRingSaveGameFolder, "sl2");
                var tempCoopSaveGames = GetTempSaves(eldenRingSaveGameFolder, "co2");

                foreach (var file in tempSaveGames)
                {
                    var newExtension = Path.GetExtension(file).Replace("temp", "");
                    _fileSystem.File.Move(file, Path.ChangeExtension(file, newExtension));
                }

                foreach (var file in tempCoopSaveGames)
                {
                    var newExtension = Path.GetExtension(file).Replace("temp", "");
                    _fileSystem.File.Move(file, Path.ChangeExtension(file, newExtension));
                }
            }
        }

        public void CreateUnmoddedBackups()
        {
            var eldenRingSaveGameFolder = GetEldenRingSavesFolder();
            if (eldenRingSaveGameFolder is null)
            {
                return;
            }

            var unmoddedBackups = Path.Combine(
                _backupsRoot,
                _baseGameBackupsFolderName);

            if (_fileSystem.Directory.Exists(eldenRingSaveGameFolder))
            {
                _fileSystem.Directory.CreateDirectory(unmoddedBackups);

                var saveGames = _fileSystem.Directory.GetFiles(eldenRingSaveGameFolder);

                foreach (var file in saveGames)
                {
                    var fileName = Path.GetFileName(file);
                    _fileSystem.File.Copy(file, $"{unmoddedBackups}\\{fileName}", true);
                }
            }
        }

        public void CreateBackups(string profileId)
        {
            var timestamp = DateTime.Now.ToString("dd_M_yyyy  H_mm_ss");

            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            var profileBackups = Path.Combine(
                _backupsRoot,
                profileId);

            var datedProfileBackupFolderPath = Path.Combine(
                profileBackups,
                timestamp);

            if (_fileSystem.Directory.Exists(profileBackups))
            {
                var allBackups = _fileSystem.Directory.GetDirectories(profileBackups);
                var ordered = allBackups.OrderBy(x => DateTime.ParseExact(
                    new DirectoryInfo(x).Name, "dd_M_yyyy  H_mm_ss", null));

                foreach (var backup in ordered.SkipLast(5))
                {
                    _fileSystem.Directory.Delete(backup, true);
                }
            }

            if (_fileSystem.Directory.Exists(profileSaves))
            {
                _fileSystem.Directory.CreateDirectory(datedProfileBackupFolderPath);

                var saveGames = _fileSystem.Directory.GetFiles(profileSaves);

                foreach (var file in saveGames)
                {
                    var fileName = Path.GetFileName(file);
                    _fileSystem.File.Copy(file, $"{datedProfileBackupFolderPath}\\{fileName}");
                }
            }
        }

        public void CopySaves(
            string oldProfileName,
            string newProfileName)
        {
            var oldProfileSaves = Path.Combine(
                _savesRoot,
                oldProfileName);

            var oldProfileBackups = Path.Combine(
                _backupsRoot,
                oldProfileName);

            var newProfileSaves = Path.Combine(
                _savesRoot,
                newProfileName);

            var newProfileBackups = Path.Combine(
                _backupsRoot,
                newProfileName);

            _fileSystem.Directory.CreateDirectory(newProfileSaves);

            var oldSaves = _fileSystem.Directory.GetFiles(oldProfileSaves);
            foreach (var file in oldSaves)
            {
                var newFilePath = Path.Combine(newProfileSaves, Path.GetFileName(file));
                _fileSystem.File.Copy(file, newFilePath);
            }

            var backupFolders = _fileSystem.Directory.GetDirectories(oldProfileBackups);
            foreach (var backupFolder in backupFolders)
            {
                var directoryName = new DirectoryInfo(backupFolder).Name;
                var newBackupPath = Path.Combine(newProfileBackups, directoryName);

                _fileSystem.Directory.CreateDirectory(newBackupPath);

                var backupSaves = _fileSystem.Directory.GetFiles(backupFolder);
                foreach (var file in backupSaves)
                {
                    var newFilePath = Path.Combine(newBackupPath, Path.GetFileName(file));
                    _fileSystem.File.Copy(file, newFilePath);
                }
            }
        }

        public void DeleteSaves(string profileId)
        {
            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            var profileBackups = Path.Combine(
                _backupsRoot,
                profileId);

            if (_fileSystem.Directory.Exists(profileSaves))
            {
                _fileSystem.Directory.Delete(profileSaves, true);
            }

            if (_fileSystem.Directory.Exists(profileBackups))
            {
                _fileSystem.Directory.Delete(profileBackups, true);
            }
        }

        public void OpenProfileSavesFolder(string profileId)
        {
            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            if (_fileSystem.Directory.Exists(profileSaves))
            {
                Process.Start("explorer", profileSaves);
            }
        }

        public bool ProfileHasSaves(string profileId)
        {
            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            return _fileSystem.Directory.Exists(profileSaves) && _fileSystem.Directory.GetFiles(profileSaves).Any();
        }

        public void ImportSave(string profileId)
        {
            var importFolder = _dialogService.ShowFolderDialog("Select saves folder to import", _savesRoot);
            if (string.IsNullOrEmpty(importFolder) || !_fileSystem.Directory.Exists(importFolder))
            {
                return;
            }

            var sl2Saves = GetSaves(importFolder, "sl2");
            var coopSaves = GetSaves(importFolder, "co2");
            var allSaves = sl2Saves.Concat(coopSaves);

            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            if (!_fileSystem.Directory.Exists(profileSaves))
            {
                _fileSystem.Directory.CreateDirectory(profileSaves);
            }

            foreach (var save in allSaves)
            {
                var fileName = Path.GetFileName(save);
                var newLocation = Path.Combine(profileSaves, fileName);
                _fileSystem.File.Copy(save, newLocation, true);
            }
        }

        public void BackupSaves(string profileId)
        {
            var exportFolder = _dialogService.ShowFolderDialog("Select folder to export to");
            if (string.IsNullOrEmpty(exportFolder) || !_fileSystem.Directory.Exists(exportFolder))
            {
                return;
            }

            var backupPath = Path.Combine(exportFolder, profileId);
            if (!_fileSystem.Directory.Exists(backupPath))
            {
                _fileSystem.Directory.CreateDirectory(backupPath);
            }

            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            var sl2Saves = GetSaves(profileSaves, "sl2");
            var coopSaves = GetSaves(profileSaves, "co2");
            var allSaves = sl2Saves.Concat(coopSaves);

            foreach (var save in allSaves)
            {
                var fileName = Path.GetFileName(save);
                var newLocation = Path.Combine(backupPath, fileName);
                _fileSystem.File.Copy(save, newLocation, true);
            }
        }

        private string? GetEldenRingSavesFolder()
        {
            var eldenRingSaveGameFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EldenRing");

            return _fileSystem.Directory.GetDirectories(eldenRingSaveGameFolder).FirstOrDefault();
        }

        private List<string> GetSaves(string directory, string extension)
        {
            return _fileSystem.Directory.GetFiles(directory, $"*.{extension}")
                .ToList()
                .Concat(_fileSystem.Directory.GetFiles(directory, $"*.{extension}.bak"))
                .ToList();
        }

        private List<string> GetTempSaves(string directory, string extension)
        {
            return _fileSystem.Directory.GetFiles(directory, $"*.{extension}temp")
                .ToList()
                .Concat(_fileSystem.Directory.GetFiles(directory, $"*.{extension}.baktemp"))
                .ToList();
        }

        public void BackupVanilla()
        {
            throw new NotImplementedException();
        }

        public void BackupProfile(IProfileVm profile)
        {
            throw new NotImplementedException();
        }

        public void Push(IProfileVm profile)
        {
            throw new NotImplementedException();
        }

        public void Pop(IProfileVm profile)
        {
            throw new NotImplementedException();
        }
    }
}
