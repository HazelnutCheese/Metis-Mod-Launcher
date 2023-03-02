using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Services.Interfaces;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services
{
    public class DllManagerService : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly DialogService _dialogService;
        private readonly IEqualityComparer<DllVm> _dllVmEqualityComparer;

        private ObservableCollection<DllVm> _dllVms;

        public ObservableCollection<DllVm> DllVms 
        { 
            get => _dllVms; 
            private set => _dllVms = value; 
        }

        public DllManagerService(
            IDatabaseService databaseService,
            IDispatcherService dispatcherService,
            ProfileManagerService profileManagerService,
            DialogService dialogService)
        {
            _databaseService = databaseService;
            _dispatcherService = dispatcherService;
            _profileManagerService = profileManagerService;
            _dialogService = dialogService;

            _dllVmEqualityComparer = new DllVmEqualityComparer();

            var dllVms = GetDllsFromDatabase(_databaseService);
            _dllVms = new ObservableCollection<DllVm>(dllVms);
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
            var dllPath = _dialogService.ShowOpenFileDialog(
                "Select External Dll",
                "Dll files (*.dll)|*.dll|All files (*.*)|*.*");

            if (dllPath is null)
            {
                return null;
            }

            var newDllVm = new DllVm(
                dllPath,
                _databaseService);

            _databaseService.AddDll(newDllVm);
            await RefreshAsync();

            return DllVms.Single(x => _dllVmEqualityComparer.Equals(x, newDllVm));
        }

        public async Task<DllVm> CopyDllAsync(DllVm dllVm)
        {
            var newDllVm = new DllVm(
                dllVm.Name + " - Copy",
                _databaseService);

            _databaseService.AddDll(newDllVm);

            newDllVm.ImagePath = dllVm.ImagePath;
            newDllVm.Description = dllVm.Description;
            newDllVm.FilePath = dllVm.FilePath;

            await RefreshAsync();

            return DllVms.Single(x => _dllVmEqualityComparer.Equals(x, newDllVm));
        }

        public async Task RemoveDllAsync(DllVm dllVm)
        {
            _databaseService.DeleteDll(dllVm);
            await RefreshAsync();
        }

        private List<DllVm> GetDllsFromDatabase(IDatabaseService databaseService)
        {
            var dlls = databaseService.GetDlls();
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
    }
}
