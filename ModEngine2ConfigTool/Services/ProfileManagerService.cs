using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services
{
    public class ProfileManagerService : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IDispatcherService _dispatcherService;
        private readonly IEqualityComparer<ProfileVm> _profileVmEqualityComparer;

        private ObservableCollection<ProfileVm> _profileVms;

        public ObservableCollection<ProfileVm> ProfileVms 
        { 
            get => _profileVms; 
            private set => _profileVms = value; 
        }

        public ProfileManagerService(
            IDatabaseService databaseService,
            IDispatcherService dispatcherService)
        {
            _databaseService = databaseService;
            _dispatcherService = dispatcherService;
            _profileVmEqualityComparer = new ProfileVmEqualityComparer();

            var profileVms = GetProfilesFromDatabase(_databaseService);
            _profileVms = new ObservableCollection<ProfileVm>(profileVms);
        }

        public async Task RefreshAsync()
        {
            var profileVms = GetProfilesFromDatabase(_databaseService);
            await _dispatcherService.InvokeUiAsync(() =>
            {
                ProfileVms.Clear();

                foreach(var profileVm in profileVms)
                {
                    ProfileVms.Add(profileVm);
                }
            });
        }

        public async Task<ProfileVm> CreateNewProfileAsync(string name)
        {
            var newProfileVm = new ProfileVm(
                name,
                _databaseService,
                _dispatcherService);

            await AddProfileAsync(newProfileVm);

            return ProfileVms.Single(x => _profileVmEqualityComparer.Equals(x, newProfileVm));
        }

        public async Task AddProfileAsync(ProfileVm profileVm)
        {
            _databaseService.AddProfile(profileVm);
            await RefreshAsync();
        }

        public async Task<ProfileVm> DuplicateProfileAsync(ProfileVm profileVm)
        {
            var newProfileVm = new ProfileVm(
                profileVm.Name + " - Copy",
                _databaseService,
                _dispatcherService);

            await AddProfileAsync(newProfileVm);

            newProfileVm.ImagePath = profileVm.ImagePath;
            newProfileVm.Description = profileVm.Description;

            newProfileVm.UseSaveManager = profileVm.UseSaveManager;
            newProfileVm.UseDebugMode = profileVm.UseDebugMode;
            newProfileVm.UseScyllaHide = profileVm.UseScyllaHide;

            foreach (var modVm in profileVm.Mods)
            {
                await AddModToProfile(newProfileVm, modVm);
            }

            foreach(var dllVm in profileVm.ExternalDlls)
            {
                await AddDllToProfile(newProfileVm, dllVm);
            }

            await RefreshAsync();

            return ProfileVms.Single(x => _profileVmEqualityComparer.Equals(x, newProfileVm));
        }

        public async Task RemoveProfileAsync(ProfileVm profileVm)
        {
            _databaseService.DeleteProfile(profileVm);
            await RefreshAsync();
        }

        public async Task AddModToProfile(ProfileVm profileVm, ModVm modVm)
        {
            if(!profileVm.Mods.Contains(modVm, new ModVmEqualityComparer()))
            {
                _databaseService.AddModToProfile(profileVm, modVm);
            }

            await profileVm.RefreshAsync();
            await RefreshAsync();
        }

        public async Task MoveModInProfile(ProfileVm profileVm, ModVm modVm, int changeAmount)
        {
            if (profileVm.Mods.Contains(modVm, new ModVmEqualityComparer()))
            {
                _databaseService.MoveModInProfile(profileVm, modVm, changeAmount);
            }

            await profileVm.RefreshAsync();
            await RefreshAsync();
        }

        public async Task RemoveModFromProfile(ProfileVm profileVm, ModVm modVm)
        {
            if (profileVm.Mods.Contains(modVm, new ModVmEqualityComparer()))
            {
                _databaseService.RemoveModFromProfile(profileVm, modVm);
            }

            await profileVm.RefreshAsync();
            await RefreshAsync();
        }

        public async Task AddDllToProfile(ProfileVm profileVm, DllVm dllVm)
        {
            if (!profileVm.ExternalDlls.Contains(dllVm, new DllVmEqualityComparer()))
            {
                _databaseService.AddDllToProfile(profileVm, dllVm);
            }

            await profileVm.RefreshAsync();
            await RefreshAsync();
        }

        public async Task MoveDllInProfile(ProfileVm profileVm, DllVm dllVm, int changeAmount)
        {
            if (profileVm.ExternalDlls.Contains(dllVm, new DllVmEqualityComparer()))
            {
                _databaseService.MoveDllInProfile(profileVm, dllVm, changeAmount);
            }

            await profileVm.RefreshAsync();
            await RefreshAsync();
        }

        public async Task RemoveDllFromProfile(ProfileVm profileVm, DllVm dllVm)
        {
            if (profileVm.ExternalDlls.Contains(dllVm, new DllVmEqualityComparer()))
            {
                _databaseService.RemoveDllFromProfile(profileVm, dllVm);
            }

            await profileVm.RefreshAsync();
            await RefreshAsync();
        }

        private List<ProfileVm> GetProfilesFromDatabase(IDatabaseService databaseService)
        {
            var profiles = databaseService.GetProfiles();
            var profileVms = new List<ProfileVm>();

            foreach (var profile in profiles)
            {
                var profileVm = new ProfileVm(
                    profile,
                    _databaseService,
                    _dispatcherService);

                profileVms.Add(profileVm);
            }

            return profileVms;
        }
    }
}
