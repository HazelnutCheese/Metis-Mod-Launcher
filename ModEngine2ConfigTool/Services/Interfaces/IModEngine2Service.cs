using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services.Interfaces
{
    public interface IModEngine2Service
    {
        Process? GetProcessByName(string name);
        Process LaunchWithProfile(string tomlPath);
        Task<Process> PollForEldenRingProcess(int timeoutMs, CancellationToken cancellationToken);
    }
}