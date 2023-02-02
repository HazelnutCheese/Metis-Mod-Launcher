using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.Generic;
using System.Linq;

namespace ModEngine2ConfigTool.Services
{
    public class DatabaseService : IDatabaseService
    {
        private DatabaseContext _databaseContext;

        public DatabaseService(string dataStorage)
        {
            _databaseContext = new DatabaseContext(dataStorage);
        }

        public List<Profile> GetProfiles()
        {
            return _databaseContext.Profiles.ToList();
        }

        public List<Mod> GetMods()
        {
            return _databaseContext.Mods.ToList();
        }

        public void SaveChanges()
        {
            _databaseContext.SaveChanges();
        }

        public void AddProfile(ProfileVm profileVm)
        {
            _databaseContext.Add(profileVm.Model);
            _databaseContext.SaveChanges();
        }

        public void DeleteProfile(ProfileVm profileVm)
        {
            _databaseContext.Remove(profileVm.Model);
            _databaseContext.SaveChanges();
        }

        public void AddMod(ModVm modVm)
        {
            _databaseContext.Add(modVm.Model);
            _databaseContext.SaveChanges();
        }

        public void DeleteMod(ModVm modVm)
        {
            _databaseContext.Remove(modVm.Model);
            _databaseContext.SaveChanges();
        }

        public void AddModToProfile(Profile profile, Mod mod)
        {
            if (!profile.Mods.Contains(mod, new ModEqualityComparer()))
            {
                profile.Mods.Add(mod);
                _databaseContext.SaveChanges();
            }
        }

        public void RemoveModFromProfile(Profile profile, Mod mod)
        {
            if (profile.Mods.Contains(mod, new ModEqualityComparer()))
            {
                profile.Mods.Remove(mod);
                _databaseContext.SaveChanges();
            }
        }
    }
}
