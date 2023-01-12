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
                $"-c \"{profileFilePath}\""
            };

            return Launch(arguments);
        }

        public static Process LaunchWithoutMods()
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

            var process = Process.Start(processStartInfo);
            if (process is null)
            {
                throw new InvalidOperationException("Failed to start Elden Ring");
            }

            return process;
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
            var steamProcesses = Process.GetProcessesByName("steam");
            var result = steamProcesses.Any();

            foreach (var steamProcess in steamProcesses)
            {
                steamProcess.Dispose();
            }

            return result;
        }

        private static async Task RenameModLoaderDll()
        {
            var eldenRingGamePath = App.ConfigurationService.EldenRingGameFolder;
            
            var modLoaderDllPath = $"{eldenRingGamePath}\\modLoader.dll";
            var dinput8DllPath = $"{eldenRingGamePath}\\dinput8.dll";

            File.Copy(modLoaderDllPath, dinput8DllPath, true);

            await Task.Delay(10000);

            File.Delete(dinput8DllPath);
        }
    }
}
