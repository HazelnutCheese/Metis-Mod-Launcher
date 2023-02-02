using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.Generic;

namespace ModEngine2ConfigTool.Services
{
    public interface IDatabaseService
    {
        void AddMod(ModVm modVm);
        void AddModToProfile(Profile profile, Mod mod);
        void AddProfile(ProfileVm profileVm);
        void DeleteMod(ModVm modVm);
        void DeleteProfile(ProfileVm profileVm);
        List<Mod> GetMods();
        List<Profile> GetProfiles();
        void RemoveModFromProfile(Profile profile, Mod mod);
        void SaveChanges();
    }
}