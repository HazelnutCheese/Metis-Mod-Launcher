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

        public DateTime? LastPlayed { get; }

        public DateTime Created { get; }

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

            Created = Model.Created;
            LastPlayed = Model.LastPlayed;

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

            Created = Model.Created;
            LastPlayed = Model.LastPlayed;

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
    }
}
