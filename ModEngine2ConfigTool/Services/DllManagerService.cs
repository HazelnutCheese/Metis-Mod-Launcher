using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            ProfileManagerService profileManagerService)
        {
            _databaseService = databaseService;
            _dispatcherService = dispatcherService;
            _profileManagerService = profileManagerService;
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
            var dllPath = GetFilePath("Select Dll file", "");
            if (string.IsNullOrWhiteSpace(dllPath))
            {
                return null;
            }

            var newDllVm = new DllVm(
                dllPath,
                _databaseService);

            await AddDllAsync(newDllVm);
            return DllVms.Single(x => _dllVmEqualityComparer.Equals(x, newDllVm));
        }

        public async Task AddDllAsync(DllVm dllVm)
        {
            _databaseService.AddDll(dllVm);
            await RefreshAsync();
        }

        public async Task<DllVm> CopyDllAsync(DllVm dllVm)
        {
            var newDllVm = new DllVm(
                dllVm.Name + " - Copy",
                _databaseService);

            await AddDllAsync(newDllVm);
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

        private static string? GetFilePath(string dialogTitle, string defaultLocation)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Dll files (*.dll)|*.dll|All files (*.*)|*.*",
                Multiselect = false,
                Title = "Select External Dll",
                CheckFileExists = true,
                CheckPathExists = true,
                InitialDirectory = defaultLocation
            };

            if(fileDialog.ShowDialog().Equals(DialogResult.OK))
            {
                return fileDialog.FileName;
            }

            return null;
        }
    }
}
