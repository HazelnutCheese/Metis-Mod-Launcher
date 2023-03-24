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
        private readonly ConfigurationService _configurationService;
        private readonly string _modEngine2DefaultPath;

        public ModEngine2Service(ConfigurationService configurationService)
        {
            _configurationService = configurationService;

            _modEngine2DefaultPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..\\ModEngine2\\ModEngine-2.0.0-preview4-win64",
                "modengine2_launcher.exe");
        }

        public Process LaunchWithProfile(string tomlPath)
        {
            var arguments = new List<string>()
            {
                "-t er",
                $"-c \"{tomlPath}\""
            };

            if(_configurationService.AutoDetectEldenRing is false)
            {
                arguments.Add($"-p \"{_configurationService.EldenRingExePath}\"");
            }

            return Launch(arguments);
        }

        public async Task<Process> PollForEldenRingProcess(int timeoutMs, CancellationToken cancellationToken)
        {
            var attempts = timeoutMs / 1000;
            for(var i = 0; i < attempts; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var eldenRingProcess = GetProcessByName("eldenring");

                if(eldenRingProcess is not null)
                {
                    return eldenRingProcess;
                }

                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }

            throw new InvalidOperationException("Could not find Elden Ring Process.");
        }

        private Process Launch(List<string> arguments) 
        {
            var modEngineExe = GetModEngine2ExePath();
            if (!File.Exists(modEngineExe))
            {
                throw new InvalidOperationException(
                    $"Could not find ModEngine2 (modengine2_launcher.exe) at " +
                    $"\"{GetModEngine2ExePath()}\".");
            }

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = modEngineExe,
                Arguments = string.Join(" ", arguments),
                WorkingDirectory = GetModEngine2FolderPath()
            };

            var process = Process.Start(processStartInfo);
            if (process is null)
            {
                throw new InvalidOperationException("Failed to start ModEngine2 process");
            }

            return process;
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

        private string GetModEngine2ExePath()
        {
            return _configurationService.AutoDetectModEngine2 is true
                ? _modEngine2DefaultPath
                : _configurationService.ModEngine2ExePath;
        }

        private string GetModEngine2FolderPath()
        {
            return Path.GetDirectoryName(GetModEngine2ExePath()) ?? "";
        }
    }
}
