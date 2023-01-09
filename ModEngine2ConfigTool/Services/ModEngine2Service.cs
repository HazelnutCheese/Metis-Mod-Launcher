using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services
{
    public static class ModEngine2Service
    {
        public static Process LaunchWithProfile(string profileName)
        {
            var profileFilePath = Path.GetFullPath($".\\Profiles\\{profileName}.toml");
            if (!File.Exists(profileFilePath))
            {
                throw new FileNotFoundException($"Could not find profile at \"{profileFilePath}\"");
            }

            var arguments = new List<string>()
            {
                "-t er",
                $"-c {profileFilePath}"
            };

            return Launch(arguments);
        }

        public static void LaunchWithoutMods()
        {
            if (!IsSteamRunning())
            {
                throw new InvalidOperationException(
                    "You must be logged into Steam to launch Elden Ring");
            }

            var eldenRingGameFolder = App.ConfigurationService.EldenRingGameFolder;
            var eldenRingGameExe = $"{eldenRingGameFolder}\\eldenring.exe";

            if (!File.Exists(eldenRingGameExe))
            {
                throw new InvalidOperationException(
                    $"Could not find EldenRing (eldenring.exe) in \"{eldenRingGameFolder}\".");
            }

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = eldenRingGameExe,
                WorkingDirectory = eldenRingGameFolder
            };

            using var process = Process.Start(processStartInfo);
            if (process is null)
            {
                throw new InvalidOperationException("Failed to start Elden Ring");
            }
        }

        public static async Task WaitForEldenRingExit()
        {
            using var eldenRingProcess = GetProcessByName("eldenring");

            if(eldenRingProcess is null)
            {
                throw new InvalidOperationException(
                    "Could not find Elden Ring Process.");
            }

            await eldenRingProcess.WaitForExitAsync();
        }

        private static Process Launch(List<string> arguments) 
        {
            if(!IsSteamRunning())
            {
                throw new InvalidOperationException(
                    "You must be logged into Steam to launch Elden Ring");
            }

            var modEngineFolder = App.ConfigurationService.ModEngine2Folder;
            var modEngineExe = $"{modEngineFolder}\\modengine2_launcher.exe";
            if (!File.Exists(modEngineExe))
            {
                throw new InvalidOperationException(
                    $"Could not find ModEngine2 (modengine2_launcher.exe) in \"{modEngineFolder}\".");
            }

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = modEngineExe,
                Arguments = string.Join(" ", arguments),
                WorkingDirectory = modEngineFolder
            };

            var process = Process.Start(processStartInfo);
            if (process is null)
            {
                throw new InvalidOperationException("Failed to start ModEngine2 process");
            }

            return process;
        }

        private static bool IsSteamRunning()
        {
            using var steamProcesses = GetProcessByName("steam");

            return steamProcesses is not null;
        }

        private static Process? GetProcessByName(string name)
        {
            var processes = Process.GetProcessesByName(name);
            var result = processes.FirstOrDefault();

            foreach (var process in processes)
            {
                if(process != result)
                {
                    process.Dispose();
                }
            }

            return result;
        }
    }
}
