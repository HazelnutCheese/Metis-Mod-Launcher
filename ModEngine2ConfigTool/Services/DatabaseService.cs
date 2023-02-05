using Microsoft.EntityFrameworkCore;
using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ModEngine2ConfigTool.Services
{
    public class DatabaseService : IDatabaseService, IDisposable
    {
        private DatabaseContext _databaseContext;

        public DatabaseService(string dataStorage)
        {
            _databaseContext = new DatabaseContext(dataStorage);
            _databaseContext.Database.EnsureCreated();
        }

        public List<Profile> GetProfiles()
        {
            return _databaseContext.Profiles
                .Include(p => p.Mods)
                .Include(p => p.Dlls)
                .ToList();
        }

        public List<Mod> GetMods()
        {
            return _databaseContext.Mods.Include(m => m.Profiles).ToList();
        }

        public void SaveChanges()
        {
            _databaseContext.SaveChanges();
        }

        public void AddProfile(ProfileVm profileVm)
        {
            if (!_databaseContext.Profiles.AsEnumerable().Contains(profileVm.Model, new ProfileEqualityComparer()))
            {
                _databaseContext.Add(profileVm.Model);
                _databaseContext.SaveChanges();
            }
        }

        public void DeleteProfile(ProfileVm profileVm)
        {
            if(_databaseContext.Profiles.AsEnumerable().Contains(profileVm.Model, new ProfileEqualityComparer()))
            {
                _databaseContext.Remove(profileVm.Model);
                _databaseContext.SaveChanges();
            }
        }

        public void AddMod(ModVm modVm)
        {
            if (!_databaseContext.Mods.AsEnumerable().Contains(modVm.Model, new ModEqualityComparer()))
            {
                _databaseContext.Add(modVm.Model);
                _databaseContext.SaveChanges();
            }
        }

        public void DeleteMod(ModVm modVm)
        {
            if (_databaseContext.Mods.AsEnumerable().Contains(modVm.Model, new ModEqualityComparer()))
            {
                _databaseContext.Remove(modVm.Model);
                _databaseContext.SaveChanges();
            }
        }

        public void AddModToProfile(ProfileVm profileVm, ModVm modVm)
        {
            if (TryGetProfile(profileVm, out var profile)
                && TryGetMod(modVm, out var mod)
                && !profile.Mods.Contains(mod, new ModEqualityComparer()))
            {
                profile.Mods.Add(mod);
                _databaseContext.SaveChanges();
            }
        }

        public void MoveModInProfile(ProfileVm profileVm, ModVm modVm, int changeAmount)
        {
            if (TryGetProfile(profileVm, out var profile)
                && TryGetMod(modVm, out var mod)
                && profile.Mods.Contains(mod, new ModEqualityComparer()))
            {
                if(profile.Mods.Count < changeAmount)
                {
                    return;
                }

                var index = profile.Mods.FindIndex(x => x.ModId == mod.ModId);
                var newIndex = index + changeAmount;

                profile.Mods.Remove(mod);

                //if(newIndex > index)
                //{
                //    newIndex -= 1;
                //}
                
                if(newIndex < 0)
                {
                    newIndex = 0;
                }
                else if(newIndex > profile.Mods.Count)
                {
                    newIndex = profile.Mods.Count;
                }

                profile.Mods.Insert(newIndex, mod);

                _databaseContext.SaveChanges();
            }
        }

        public void RemoveModFromProfile(ProfileVm profileVm, ModVm modVm)
        {
            if(TryGetProfile(profileVm, out var profile) 
                && TryGetMod(modVm, out var mod)
                && profile.Mods.Contains(mod, new ModEqualityComparer()))
            {
                profile.Mods.Remove(mod); 
                _databaseContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            _databaseContext.Dispose();
        }

        private bool TryGetProfile(
            ProfileVm profileVm, 
            [NotNullWhen(returnValue: true)] out Profile? result)
        {
            result = _databaseContext.Find(
                typeof(Profile),
                profileVm.Model.ProfileId) as Profile;

            return result is not null;
        }

        private bool TryGetMod(
            ModVm modVm,
            [NotNullWhen(returnValue: true)] out Mod? result)
        {
            result = _databaseContext.Find(
                typeof(Mod),
                modVm.Model.ModId) as Mod;

            return result is not null;
        }
    }
}
