using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Equality;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.ViewModels.Profiles
{
    public class ProfileVm : ObservableObject
    {
        public Profile Model { get; private set; }

        private string _description;
        private string _name;
        private string _imagePath;
        private bool _useDebugMode;
        private bool _useScyllaHide;
        private bool _useSaveManager;
        private readonly IDatabaseService _databaseService;
        private readonly IDispatcherService _dispatcherService;

        public string Name
        {
            get => _name;
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _name, value);
                    Model.Name = value;
                    _databaseService.SaveChanges();
                }
            }
        }

        public string Description
        {
            get => _description;
            set 
            {
                SetProperty(ref _description, value);
                Model.Description = value;
                _databaseService.SaveChanges();
            }
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                SetProperty(ref _imagePath, value);
                Model.ImagePath = value;
                _databaseService.SaveChanges();
            }
        }

        public bool UseSaveManager
        {
            get => _useSaveManager;
            set
            {
                SetProperty(ref _useSaveManager, value);
                Model.UseSaveManager = value;
                _databaseService.SaveChanges();
            }
        }

        public bool UseDebugMode
        {
            get => _useDebugMode;
            set
            {
                SetProperty(ref _useDebugMode, value);
                Model.UseDebugMode = value;
                _databaseService.SaveChanges();
            }
        }

        public bool UseScyllaHide
        {
            get => _useScyllaHide;
            set
            {
                SetProperty(ref _useScyllaHide, value);
                Model.UseScyllaHide = value;
                _databaseService.SaveChanges();
            }
        }

        public DateTime? LastPlayed => Model.LastPlayed;

        public DateTime Created => Model.Created;

        public ObservableCollection<ModVm> Mods { get; }

        public ObservableCollection<DllVm> ExternalDlls { get; }

        public ProfileVm(string name, 
            IDatabaseService databaseService,
            IDispatcherService dispatcherService)
        {
            Model = new Profile()
            {
                ProfileId = Guid.NewGuid(),
                Name = name,
                Description = string.Empty,
                ImagePath = string.Empty,
                Created = DateTime.Now,
                LastPlayed = null,
                Mods = new List<Mod>(),
                Dlls = new List<Dll>()
            };
            _databaseService = databaseService;
            _dispatcherService = dispatcherService;

            _name = Model.Name;
            _description = Model.Description ?? "";
            _imagePath = Model.ImagePath ?? "";

            Mods = new ObservableCollection<ModVm>();
            ExternalDlls= new ObservableCollection<DllVm>();
        }

        public ProfileVm(
            Profile profile, 
            IDatabaseService databaseService,
            IDispatcherService dispatcherService)
        {
            Model = profile;
            _databaseService = databaseService;
            _dispatcherService = dispatcherService;
            _name = Model.Name;
            _description = Model.Description ?? "";
            _imagePath = Model.ImagePath ?? "";

            Mods = new ObservableCollection<ModVm>(
                Model.Mods.Select(x => new ModVm(x, _databaseService)));

            ExternalDlls = new ObservableCollection<DllVm>(
                Model.Dlls.Select(x => new DllVm(x, _databaseService)));
        }

        public async Task RefreshAsync()
        {
            Model = _databaseService
                .GetProfiles()
                .Single(x => new ProfileEqualityComparer().Equals(x, Model));

            _name = Model.Name;
            _description = Model.Description ?? "";
            _imagePath = Model.ImagePath ?? "";

            var modVms = Model.Mods.Select(x => new ModVm(x, _databaseService));
            var dllVms = Model.Dlls.Select(x => new DllVm(x, _databaseService));

            await _dispatcherService.InvokeUiAsync(() =>
            {
                Mods.Clear();
                foreach (var modVm in modVms)
                {
                    Mods.Add(modVm);
                }

                ExternalDlls.Clear();
                foreach (var dllVm in dllVms)
                {
                    ExternalDlls.Add(dllVm);
                }
            });
        }

        public void UpdateLastPlayed()
        {
            Model.LastPlayed = DateTime.Now;
            _databaseService.SaveChanges();
            OnPropertyChanged(nameof(LastPlayed));
        }
    }
}
