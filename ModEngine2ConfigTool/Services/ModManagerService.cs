﻿using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Services.Interfaces;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.Services
{
    public class ModManagerService : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly DialogService _dialogService;
        private readonly IEqualityComparer<ModVm> _modVmEqualityComparer;

        private ObservableCollection<ModVm> _modVms;

        public ObservableCollection<ModVm> ModVms 
        { 
            get => _modVms; 
            private set => _modVms = value; 
        }

        public ModManagerService(
            IDatabaseService databaseService,
            IDispatcherService dispatcherService,
            ProfileManagerService profileManagerService,
            DialogService dialogService)
        {
            _databaseService = databaseService;
            _dispatcherService = dispatcherService;
            _profileManagerService = profileManagerService;
            _dialogService = dialogService;

            _modVmEqualityComparer = new ModVmEqualityComparer();

            var modVms = GetModsFromDatabase(_databaseService);
            _modVms = new ObservableCollection<ModVm>(modVms);
        }

        public async Task RefreshAsync()
        {
            var modVms = GetModsFromDatabase(_databaseService);
            await _dispatcherService.InvokeUiAsync(() =>
            {
                ModVms.Clear();

                foreach(var modVm in modVms)
                {
                    ModVms.Add(modVm);
                }
            });

            await _profileManagerService.RefreshAsync();
        }

        public async Task<ModVm?> ImportModAsync()
        {
            var modPath = _dialogService.ShowFolderDialog("Select Mod Folder");
            if (modPath is null)
            {
                return null;
            }

            var newModVm = new ModVm(
                modPath,
                _databaseService);

            _databaseService.AddMod(newModVm);

            await RefreshAsync();

            return ModVms.Single(x => _modVmEqualityComparer.Equals(x, newModVm));
        }

        public async Task<ModVm> DuplicateModAsync(ModVm modVm)
        {
            var newModVm = new ModVm(
                modVm.Name + " - Copy",
                _databaseService);

            _databaseService.AddMod(newModVm);

            newModVm.Description = modVm.Description;
            newModVm.FolderPath = modVm.FolderPath;
            newModVm.ImagePath = modVm.ImagePath;

            await RefreshAsync();

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
    }
}
