using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModEngine2ConfigTool.Services
{
    public class DllManagerService : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly IEqualityComparer<ModVm> _modVmEqualityComparer;

        private ObservableCollection<DllVm> _dllVms;

        public ObservableCollection<DllVm> DllVms 
        { 
            get => _dllVms; 
            private set => _dllVms = value; 
        }

        public DllManagerService(
            IDatabaseService databaseService,
            IDispatcherService dispatcherService,
            ProfileManagerService profileManagerService)
        {
            _databaseService = databaseService;
            _dispatcherService = dispatcherService;
            _profileManagerService = profileManagerService;
            _modVmEqualityComparer = new ModVmEqualityComparer();

            var dllVms = GetModsFromDatabase(_databaseService);
            _dllVms = new ObservableCollection<ModVm>(dllVms);
        }

        public async Task RefreshAsync()
        {
            var dllVms = GetDllsFromDatabase(_databaseService);
            await _dispatcherService.InvokeUiAsync(() =>
            {
                DllVms.Clear();

                foreach(var dllVm in dllVms)
                {
                    DllVms.Add(dllVm);
                }
            });

            await _profileManagerService.RefreshAsync();
        }

        public async Task<DllVm?> ImportDllAsync()
        {
            var modPath = GetFolderPath("Select Mod folder", "");
            if (string.IsNullOrWhiteSpace(modPath))
            {
                return null;
            }

            var newModVm = new ModVm(
                modPath,
                _databaseService);

            await AddModAsync(newModVm);
            return ModVms.Single(x => _modVmEqualityComparer.Equals(x, newModVm));
        }

        public async Task AddModAsync(ModVm modVm)
        {
            _databaseService.AddMod(modVm);
            await RefreshAsync();
        }

        public async Task<ModVm> DuplicateModAsync(ModVm modVm)
        {
            var newModVm = new ModVm(
                modVm.Name + " - Copy",
                _databaseService);

            await AddModAsync(newModVm);
            return ModVms.Single(x => _modVmEqualityComparer.Equals(x, newModVm));
        }

        public async Task RemoveModAsync(ModVm modVm)
        {
            _databaseService.DeleteMod(modVm);
            await RefreshAsync();
        }

        private List<ModVm> GetModsFromDatabase(IDatabaseService databaseService)
        {
            var mods = databaseService.GetMods();
            var modVms = new List<ModVm>();

            foreach (var mod in mods)
            {
                var modVm = new ModVm(
                    mod,
                    _databaseService);

                modVms.Add(modVm);
            }

            return modVms;
        }

        private List<DllVm> GetDllsFromDatabase(IDatabaseService databaseService)
        {
            var dlls = databaseService.GetMods();
            var dllVms = new List<DllVm>();

            foreach (var dll in dlls)
            {
                var dllVm = new DllVm(
                    dll,
                    _databaseService);

                dllVms.Add(dllVm);
            }

            return dllVms;
        }

        private static string? GetFolderPath(string dialogTitle, string defaultLocation)
        {
            var dialog = new FolderBrowserEx.FolderBrowserDialog
            {
                Title = dialogTitle,
                InitialFolder = @"C:\",
                AllowMultiSelect = false
            };

            if (string.IsNullOrWhiteSpace(defaultLocation) || !Directory.Exists(defaultLocation))
            {
                dialog.InitialFolder = "C:\\";
            }
            else
            {
                dialog.InitialFolder = defaultLocation;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedFolder;
            }
            else
            {
                return null;
            }
        }
    }
}
