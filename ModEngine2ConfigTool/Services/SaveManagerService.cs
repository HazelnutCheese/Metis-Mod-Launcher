using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ModEngine2ConfigTool.Services
{
    public class SaveManagerService
    {
        private const string _tempExt = "mtemp";
        private const string _savesFolderName = "Saves";
        private const string _backupsFolderName = "Backups";
        private const string _baseGameBackupsFolderName = "Vanilla";

        private readonly string _savesRoot;

        private readonly string _backupsRoot;
        private readonly ConfigurationService _configurationService;
        private readonly DialogService _dialogService;

        public SaveManagerService(
            string dataStorage, 
            ConfigurationService configurationService, 
            DialogService dialogService)
        {
            _savesRoot = Path.Combine(dataStorage, _savesFolderName);
            _backupsRoot = Path.Combine(_savesRoot, _backupsFolderName);

            if(!Directory.Exists(_savesRoot))
            {
                Directory.CreateDirectory(_savesRoot);
            }

            if(!Directory.Exists(_backupsRoot))
            {
                Directory.CreateDirectory(_backupsRoot);
            }

            _configurationService = configurationService;
            _dialogService = dialogService;
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

            if (Directory.Exists(eldenRingSaveGameFolder))
            {
                var existingSaveGames = GetSaves(eldenRingSaveGameFolder);

                foreach (var file in existingSaveGames)
                {
                    var newExtension = $"{Path.GetExtension(file)}{_tempExt}";
                    File.Move(file, Path.ChangeExtension(file, newExtension), true);
                }
            }

            if (Directory.Exists(profileSaves))
            {
                var existingSaveGames = GetSaves(profileSaves);

                foreach (var file in existingSaveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Copy(file, $"{eldenRingSaveGameFolder}\\{fileName}");
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

            Directory.CreateDirectory(profileSaves);

            if (Directory.Exists(eldenRingSaveGameFolder))
            {
                var existingSaveGames = GetSaves(eldenRingSaveGameFolder);

                foreach (var file in existingSaveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Move(file, $"{profileSaves}\\{fileName}", true);
                }

                var tempSaveGames = GetTempSaves(eldenRingSaveGameFolder);

                foreach (var file in tempSaveGames)
                {
                    var newExtension = Path.GetExtension(file).Replace(_tempExt, "");
                    File.Move(file, Path.ChangeExtension(file, newExtension));
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

            var timestamp = DateTime.Now.ToString("dd_M_yyyy  H_mm_ss");

            var unmoddedBackups = Path.Combine(
                _backupsRoot,
                _baseGameBackupsFolderName);

            var datedProfileBackupFolderPath = Path.Combine(
                unmoddedBackups,
                timestamp);

            if (Directory.Exists(unmoddedBackups))
            {
                var allBackups = Directory.GetDirectories(unmoddedBackups);
                var ordered = allBackups.OrderBy(x => DateTime.ParseExact(
                    new DirectoryInfo(x).Name, "dd_M_yyyy  H_mm_ss", null));

                foreach (var backup in ordered.SkipLast(10))
                {
                    Directory.Delete(backup, true);
                }
            }

            if (Directory.Exists(eldenRingSaveGameFolder))
            {
                Directory.CreateDirectory(datedProfileBackupFolderPath);

                var saveGames = Directory.GetFiles(eldenRingSaveGameFolder);

                foreach (var file in saveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Copy(file, $"{datedProfileBackupFolderPath}\\{fileName}", true);
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

            if(Directory.Exists(profileBackups))
            {
                var allBackups = Directory.GetDirectories(profileBackups);
                var ordered = allBackups.OrderBy(x => DateTime.ParseExact(
                    new DirectoryInfo(x).Name, "dd_M_yyyy  H_mm_ss", null));

                foreach (var backup in ordered.SkipLast(5))
                {
                    Directory.Delete(backup, true);
                }
            }

            if (Directory.Exists(profileSaves))
            {
                Directory.CreateDirectory(datedProfileBackupFolderPath);

                var saveGames = Directory.GetFiles(profileSaves);

                foreach (var file in saveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Copy(file, $"{datedProfileBackupFolderPath}\\{fileName}");
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

            Directory.CreateDirectory(newProfileSaves);

            var oldSaves = Directory.GetFiles(oldProfileSaves);
            foreach(var file in oldSaves) 
            {
                var newFilePath = Path.Combine(newProfileSaves, Path.GetFileName(file));
                File.Copy(file, newFilePath);
            }

            var backupFolders = Directory.GetDirectories(oldProfileBackups);
            foreach(var backupFolder in backupFolders) 
            {
                var directoryName = new DirectoryInfo(backupFolder).Name;
                var newBackupPath = Path.Combine(newProfileBackups, directoryName);

                Directory.CreateDirectory(newBackupPath);

                var backupSaves = Directory.GetFiles(backupFolder);
                foreach (var file in backupSaves)
                {
                    var newFilePath = Path.Combine(newBackupPath, Path.GetFileName(file));
                    File.Copy(file, newFilePath);
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

            if(Directory.Exists(profileSaves))
            {
                Directory.Delete(profileSaves, true);
            }

            if (Directory.Exists(profileBackups))
            {
                Directory.Delete(profileBackups, true);
            }
        }

        public void OpenBackupSavesFolder()
        {
            if (Directory.Exists(_backupsRoot))
            {
                Process.Start("explorer", _backupsRoot);
            }
        }

        public void OpenProfileSavesFolder(string profileId)
        {
            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            if (Directory.Exists(profileSaves))
            {
                Process.Start("explorer", profileSaves);
            }
        }

        public bool ProfileHasSaves(string profileId)
        {
            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            return Directory.Exists(profileSaves) && Directory.GetFiles(profileSaves).Any();
        }

        public void ImportSave(string profileId)
        {
            if(ProfileHasSaves(profileId))
            {
                // Show are you sure dialog
            }

            var importFolder = _dialogService.ShowFolderDialog("Select saves folder to import", _savesRoot);
            if(string.IsNullOrEmpty(importFolder) || !Directory.Exists(importFolder))
            {
                return;
            }

            var allSaves = Directory.GetFiles(importFolder);

            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            if (!Directory.Exists(profileSaves))
            {
                Directory.CreateDirectory(profileSaves);
            }

            foreach (var save in allSaves)
            {
                var fileName = Path.GetFileName(save);
                var newLocation = Path.Combine(profileSaves, fileName);
                File.Copy(save, newLocation, true);
            }
        }

        public void BackupSaves(string profileId)
        {
            var exportFolder = _dialogService.ShowFolderDialog("Select folder to export to");
            if (string.IsNullOrEmpty(exportFolder) || !Directory.Exists(exportFolder))
            {
                return;
            }

            var backupPath = Path.Combine(exportFolder, profileId);
            if(!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }

            var profileSaves = Path.Combine(
                _savesRoot,
                profileId);

            var allSaves = Directory.GetFiles(profileSaves);

            foreach (var save in allSaves)
            {
                var fileName = Path.GetFileName(save);
                var newLocation = Path.Combine(backupPath, fileName);
                File.Copy(save, newLocation, true);
            }
        }

        private string? GetEldenRingSavesFolder()
        {
            if(_configurationService.AutoDetectSaves.HasValue 
                && _configurationService.AutoDetectSaves.Value.Equals(false))
            {
                return _configurationService.EldenRingSavesPath;
            }

            var eldenRingSaveGameFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EldenRing");

            return Directory.GetDirectories(eldenRingSaveGameFolder).FirstOrDefault();
        }

        private List<string> GetSaves(string folder)
        {
            return Directory.GetFiles(folder)
                .Where(x => !Path.GetExtension(x).EndsWith(_tempExt))
                .ToList();
        }

        private List<string> GetTempSaves(string folder)
        {
            return Directory.GetFiles(folder)
                .Where(x => Path.GetExtension(x).EndsWith(_tempExt))
                .ToList();
        }
    }
}
