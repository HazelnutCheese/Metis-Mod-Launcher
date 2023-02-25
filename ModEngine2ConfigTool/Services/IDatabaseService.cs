using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.Generic;

namespace ModEngine2ConfigTool.Services
{
    public interface IDatabaseService
    {
        List<Mod> GetMods();
        List<Dll> GetDlls();
        List<Profile> GetProfiles();
        void AddProfile(ProfileVm profileVm);
        void DeleteProfile(ProfileVm profileVm);

        void AddMod(ModVm modVm);
        void DeleteMod(ModVm modVm);
        void AddModToProfile(ProfileVm profile, ModVm mod);
        void MoveModInProfile(ProfileVm profileVm, ModVm modVm, int changeAmount);
        void RemoveModFromProfile(ProfileVm profile, ModVm mod);

        void AddDll(DllVm dllVm);
        void RemoveDllFromProfile(ProfileVm profileVm, DllVm dllVm);
        void MoveDllInProfile(ProfileVm profileVm, DllVm dllVm, int changeAmount);
        void AddDllToProfile(ProfileVm profileVm, DllVm dllVm);
        void DeleteDll(DllVm dllVm);

        void SaveChanges();
        void DiscardChanges();
        void AddDll(Dll dll);
        void AddMod(Mod mod);
        void AddProfile(Profile profile);
    }
}