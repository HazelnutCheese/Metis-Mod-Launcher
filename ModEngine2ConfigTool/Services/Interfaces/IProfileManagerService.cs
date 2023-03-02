using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services.Interfaces
{
    public interface IProfileManagerService
    {
        ObservableCollection<ProfileVm> ProfileVms { get; }

        Task AddDllToProfile(ProfileVm profileVm, DllVm dllVm);
        Task AddModToProfile(ProfileVm profileVm, ModVm modVm);
        Task AddProfileAsync(ProfileVm profileVm);
        Task<ProfileVm> CreateNewProfileAsync(string name);
        Task<ProfileVm> DuplicateProfileAsync(ProfileVm profileVm);
        Task MoveDllInProfile(ProfileVm profileVm, DllVm dllVm, int changeAmount);
        Task MoveModInProfile(ProfileVm profileVm, ModVm modVm, int changeAmount);
        Task RefreshAsync();
        Task RemoveDllFromProfile(ProfileVm profileVm, DllVm dllVm);
        Task RemoveModFromProfile(ProfileVm profileVm, ModVm modVm);
        Task RemoveProfileAsync(ProfileVm profileVm);
    }
}