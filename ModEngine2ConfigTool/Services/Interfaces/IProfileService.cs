using ModEngine2ConfigTool.ViewModels.Profiles;

namespace ModEngine2ConfigTool.Services.Interfaces
{
    public interface IProfileService
    {
        string WriteProfile(IProfileVm profile);
    }
}