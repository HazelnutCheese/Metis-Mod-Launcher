using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ModEngine2ConfigTool.Services
{
    public class SaveManagerService
    {
        private const string _savesFolderName = "Saves";
        private const string _backupsFolderName = "Backups";
        private const string _baseGameBackupsFolderName = "Vanilla";

        private readonly string _savesRoot;

        private readonly string _backupsRoot;

        public SaveManagerService(string dataStorage)
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
                var existingSaveGames = GetSaves(eldenRingSaveGameFolder, "sl2");
                var existingCoopSaveGames = GetSaves(eldenRingSaveGameFolder, "co2");

                foreach (var file in existingSaveGames)
                {
                    var newExtension = $"{Path.GetExtension(file)}temp";
                    File.Move(file, Path.ChangeExtension(file, newExtension));
                }

                foreach (var file in existingCoopSaveGames)
                {
                    var newExtension = $"{Path.GetExtension(file)}temp";
                    File.Move(file, Path.ChangeExtension(file, newExtension));
                }
            }

            if (Directory.Exists(profileSaves))
            {
                var existingSaveGames = GetSaves(profileSaves, "sl2");
                var existingCoopSaveGames = GetSaves(profileSaves, "co2");

                foreach (var file in existingSaveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Copy(file, $"{eldenRingSaveGameFolder}\\{fileName}");
                }

                foreach (var file in existingCoopSaveGames)
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
                var existingSaveGames = GetSaves(eldenRingSaveGameFolder, "sl2");
                var existingCoopSaveGames = GetSaves(eldenRingSaveGameFolder, "co2");

                foreach (var file in existingSaveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Move(file, $"{profileSaves}\\{fileName}", true);
                }

                foreach (var file in existingCoopSaveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Move(file, $"{profileSaves}\\{fileName}", true);
                }

                var tempSaveGames = GetTempSaves(eldenRingSaveGameFolder, "sl2");
                var tempCoopSaveGames = GetTempSaves(eldenRingSaveGameFolder, "co2");

                foreach (var file in tempSaveGames)
                {
                    var newExtension = Path.GetExtension(file).Replace("temp", "");
                    File.Move(file, Path.ChangeExtension(file, newExtension));
                }

                foreach (var file in tempCoopSaveGames)
                {
                    var newExtension = Path.GetExtension(file).Replace("temp", "");
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

            var unmoddedBackups = Path.Combine(
                _backupsRoot,
                _baseGameBackupsFolderName);

            if (Directory.Exists(eldenRingSaveGameFolder))
            {
                Directory.CreateDirectory(unmoddedBackups);

                var saveGames = Directory.GetFiles(eldenRingSaveGameFolder);

                foreach (var file in saveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Copy(file, $"{unmoddedBackups}\\{fileName}", true);
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

            var importFolder = GetFolderPath("Select saves folder to import", _savesRoot);
            if(string.IsNullOrEmpty(importFolder) || !Directory.Exists(importFolder))
            {
                return;
            }

            var sl2Saves = GetSaves(importFolder, "sl2");
            var coopSaves = GetSaves(importFolder, "co2");
            var allSaves = sl2Saves.Concat(coopSaves);

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
            var exportFolder = GetFolderPath("Select folder to export to", "");
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

            var sl2Saves = GetSaves(profileSaves, "sl2");
            var coopSaves = GetSaves(profileSaves, "co2");
            var allSaves = sl2Saves.Concat(coopSaves);

            foreach (var save in allSaves)
            {
                var fileName = Path.GetFileName(save);
                var newLocation = Path.Combine(backupPath, fileName);
                File.Copy(save, newLocation, true);
            }
        }

        private string? GetEldenRingSavesFolder()
        {
            var eldenRingSaveGameFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "EldenRing");

            return Directory.GetDirectories(eldenRingSaveGameFolder).FirstOrDefault();
        }

        private static List<string> GetSaves(string directory, string extension)
        {
            return Directory.GetFiles(directory, $"*.{extension}")
                .ToList()
                .Concat(Directory.GetFiles(directory, $"*.{extension}.bak"))
                .ToList();
        }

        private static List<string> GetTempSaves(string directory, string extension)
        {
            return Directory.GetFiles(directory, $"*.{extension}temp")
                .ToList()
                .Concat(Directory.GetFiles(directory, $"*.{extension}.baktemp"))
                .ToList();
        }

        private static string? GetFolderPath(string dialogTitle, string defaultLocation)
        {
            var dialog = new FolderBrowserEx.FolderBrowserDialog
            {
                Title = dialogTitle,
                InitialFolder = @"C:\",
                AllowMultiSelect = false
            };

            if (string.IsNullOrWhiteSpace(defaultLocation) || !Directory.Exists(defaultLocation))
            {
                dialog.InitialFolder = "C:\\";
            }
            else
            {
                dialog.InitialFolder = defaultLocation;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedFolder;
            }
            else
            {
                return null;
            }
        }
    }
}
