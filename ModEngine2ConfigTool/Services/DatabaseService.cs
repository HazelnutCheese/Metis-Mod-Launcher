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
        private readonly string _dataStorage;

        public DatabaseService(string dataStorage)
        {            
            _dataStorage = dataStorage;
            _databaseContext = new DatabaseContext(_dataStorage);
            _databaseContext.Database.EnsureCreated();
            _databaseContext.Database.Migrate();
        }

        public List<Profile> GetProfiles()
        {
            var profiles = _databaseContext.Profiles
                .Include(p => p.Mods)
                .Include(p => p.Dlls)
                .ToList();

            foreach(var profile in profiles)
            {
                if(!string.IsNullOrEmpty(profile.ModsOrder))
                {
                    var order = profile.ModsOrder.Split(";", StringSplitOptions.None);

                    profile.Mods = profile
                        .Mods
                        .OrderBy(x => Array.IndexOf(order, x.ModId.ToString())).ToList();
                }

                if (!string.IsNullOrEmpty(profile.DllsOrder))
                {
                    var order = profile.DllsOrder.Split(";", StringSplitOptions.None);

                    profile.Dlls = profile
                        .Dlls
                        .OrderBy(x => Array.IndexOf(order, x.DllId.ToString())).ToList();
                }
            }

            return profiles;
        }

        public List<Mod> GetMods()
        {
            return _databaseContext.Mods.Include(m => m.Profiles).ToList();
        }

        public List<Dll> GetDlls()
        {
            return _databaseContext.Dlls.Include(m => m.Profiles).ToList();
        }

        public void SaveChanges()
        {
            _databaseContext.SaveChanges();
        }

        public void DiscardChanges()
        {
            _databaseContext.Dispose();
            _databaseContext = new DatabaseContext(_dataStorage);
        }

        public void AddProfile(ProfileVm profileVm)
        {
            if (!_databaseContext.Profiles.AsEnumerable().Contains(profileVm.Model, new ProfileEqualityComparer()))
            {
                SortOrdering(profileVm.Model);

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

                SortOrdering(profile);

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
               
                if(newIndex < 0)
                {
                    newIndex = 0;
                }
                else if(newIndex > profile.Mods.Count)
                {
                    newIndex = profile.Mods.Count;
                }

                profile.Mods.Insert(newIndex, mod);

                SortOrdering(profile);

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

                SortOrdering(profile);

                _databaseContext.SaveChanges();
            }
        }

        public void AddDll(DllVm dllVm)
        {
            if (!_databaseContext.Dlls.AsEnumerable().Contains(dllVm.Model, new DllEqualityComparer()))
            {
                _databaseContext.Add(dllVm.Model);
                _databaseContext.SaveChanges();
            }
        }

        public void DeleteDll(DllVm dllVm)
        {
            if (_databaseContext.Dlls.AsEnumerable().Contains(dllVm.Model, new DllEqualityComparer()))
            {
                _databaseContext.Remove(dllVm.Model);
                _databaseContext.SaveChanges();
            }
        }

        public void AddDllToProfile(ProfileVm profileVm, DllVm dllVm)
        {
            if (TryGetProfile(profileVm, out var profile)
                && TryGetDll(dllVm, out var dll)
                && !profile.Dlls.Contains(dll, new DllEqualityComparer()))
            {
                profile.Dlls.Add(dll);

                SortOrdering(profile);

                _databaseContext.SaveChanges();
            }
        }

        public void MoveDllInProfile(ProfileVm profileVm, DllVm dllVm, int changeAmount)
        {
            if (TryGetProfile(profileVm, out var profile)
                && TryGetDll(dllVm, out var dll)
                && profile.Dlls.Contains(dll, new DllEqualityComparer()))
            {
                if (profile.Dlls.Count < changeAmount)
                {
                    return;
                }

                var index = profile.Dlls.FindIndex(x => x.DllId == dll.DllId);
                var newIndex = index + changeAmount;

                profile.Dlls.Remove(dll);

                if (newIndex < 0)
                {
                    newIndex = 0;
                }
                else if (newIndex > profile.Dlls.Count)
                {
                    newIndex = profile.Dlls.Count;
                }

                profile.Dlls.Insert(newIndex, dll);

                SortOrdering(profile);

                _databaseContext.SaveChanges();
            }
        }

        public void RemoveDllFromProfile(ProfileVm profileVm, DllVm dllVm)
        {
            if (TryGetProfile(profileVm, out var profile)
                && TryGetDll(dllVm, out var dll)
                && profile.Dlls.Contains(dll, new DllEqualityComparer()))
            {
                profile.Dlls.Remove(dll);

                SortOrdering(profile);

                _databaseContext.SaveChanges();
            }
        }

        public void AddProfile(Profile profile)
        {
            _databaseContext.Add(profile);
        }

        public void AddMod(Mod mod)
        {
            _databaseContext.Add(mod);
        }

        public void AddDll(Dll dll)
        {
            _databaseContext.Add(dll);
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

        private bool TryGetDll(
            DllVm dllVm,
            [NotNullWhen(returnValue: true)] out Dll? result)
        {
            result = _databaseContext.Find(
                typeof(Dll),
                dllVm.Model.DllId) as Dll;

            return result is not null;
        }

        private void SortOrdering(Profile profile)
        {
            profile.ModsOrder = string.Join(";", profile.Mods.Select(x => x.ModId));
            profile.DllsOrder = string.Join(";", profile.Dlls.Select(x => x.DllId));
        }
    }
}
