using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services
{
    public class ModEngine2Service
    {
        private readonly string _modEngine2Folder;

        public ModEngine2Service(string modEngine2Folder)
        {
            _modEngine2Folder = modEngine2Folder;
        }

        public Process LaunchWithProfile(string tomlPath)
        {
            var arguments = new List<string>()
            {
                "-t er",
                $"-c \"{tomlPath}\""
            };

            return Launch(arguments);
        }

        public async Task WaitForEldenRingExit(CancellationToken cancellationToken)
        {
            using var eldenRingProcess = GetProcessByName("eldenring");

            if(eldenRingProcess is null)
            {
                throw new InvalidOperationException(
                    "Could not find Elden Ring Process.");
            }

            try
            {
                await eldenRingProcess.WaitForExitAsync(cancellationToken);
            }
            catch(TaskCanceledException)
            {
                eldenRingProcess.Kill();
            }
        }

        private Process Launch(List<string> arguments) 
        {
            if(!IsSteamRunning())
            {
                throw new InvalidOperationException(
                    "You must be logged into Steam to launch Elden Ring");
            }

            var modEngineExe = $"{_modEngine2Folder}\\modengine2_launcher.exe";
            if (!File.Exists(modEngineExe))
            {
                throw new InvalidOperationException(
                    $"Could not find ModEngine2 (modengine2_launcher.exe) in \"{_modEngine2Folder}\".");
            }

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = modEngineExe,
                Arguments = string.Join(" ", arguments),
                WorkingDirectory = _modEngine2Folder
            };

            var process = Process.Start(processStartInfo);
            if (process is null)
            {
                throw new InvalidOperationException("Failed to start ModEngine2 process");
            }

            return process;
        }

        private bool IsSteamRunning()
        {
            using var steamProcesses = GetProcessByName("steam");

            return steamProcesses is not null;
        }

        public Process? GetProcessByName(string name)
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
