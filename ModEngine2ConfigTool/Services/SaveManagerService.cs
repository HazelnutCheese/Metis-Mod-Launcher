using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModEngine2ConfigTool.Services
{
    public static class SaveManagerService
    {
        private const string _savesFolderName = "Saves";
        private const string _backupsFolderName = "Backups";
        private const string _baseGameBackupsFolderName = "Unmodded";

        private static string SavesRoot { get; } = Path.Combine(
            Directory.GetCurrentDirectory(),
            _savesFolderName);

        private static string BackupsRoot { get; } = Path.Combine(
            SavesRoot,
            _backupsFolderName);

        public static void InstallProfileSaves(string profileName)
        {
            var eldenRingSaveGameFolder = App.ConfigurationService.SaveGameFolder;

            var profileSaves = Path.Combine(
                SavesRoot,
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

        public static void UninstallProfileSaves(string profileName)
        {
            var eldenRingSaveGameFolder = App.ConfigurationService.SaveGameFolder;

            var profileSaves = Path.Combine(
                SavesRoot,
                profileName);

            if (!Directory.Exists(profileSaves)) 
            { 
                Directory.CreateDirectory(profileSaves);
            }

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

        public static void CreateUnmoddedBackups()
        {
            var eldenRingSaveGameFolder = App.ConfigurationService.SaveGameFolder;

            var unmoddedBackups = Path.Combine(
                BackupsRoot,
                _baseGameBackupsFolderName);

            Directory.CreateDirectory(unmoddedBackups);

            if (Directory.Exists(eldenRingSaveGameFolder))
            {
                var saveGames = Directory.GetFiles(eldenRingSaveGameFolder);

                foreach (var file in saveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Copy(file, $"{unmoddedBackups}\\{fileName}", true);
                }
            }
        }

        public static void CreateBackups(string profileName)
        {
            var timestamp = DateTime.Now.ToString("dd_M_yyyy  H_mm_ss");

            var profileSaves = Path.Combine(
                SavesRoot,
                profileName);

            var profileBackups = Path.Combine(
                BackupsRoot,
                profileName);

            var datedProfileBackupFolderPath = Path.Combine(
                profileBackups,
                timestamp);

            Directory.CreateDirectory(profileSaves);
            Directory.CreateDirectory(datedProfileBackupFolderPath);

            if (Directory.Exists(profileSaves))
            {
                var saveGames = Directory.GetFiles(profileSaves);

                foreach (var file in saveGames)
                {
                    var fileName = Path.GetFileName(file);
                    File.Copy(file, $"{datedProfileBackupFolderPath}\\{fileName}");
                }
            }
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
    }
}
