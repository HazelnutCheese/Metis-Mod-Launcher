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

        private readonly IDatabaseService _databaseService;
        private readonly IDispatcherService _dispatcherService;

        public string Name
        {
            get => Model.Name ?? string.Empty;
            set
            {
                if (Model.Name != value)
                {
                    Model.Name = value;
                    _databaseService.SaveChanges();
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => Model.Description ?? string.Empty;
            set
            {
                if (Model.Description != value)
                {
                    Model.Description = value;
                    _databaseService.SaveChanges();
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string ImagePath
        {
            get => Model.ImagePath ?? string.Empty;
            set
            {
                if (Model.ImagePath != value)
                {
                    Model.ImagePath = value;
                    _databaseService.SaveChanges();
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public bool UseSaveManager
        {
            get => Model.UseSaveManager;
            set
            {
                if (Model.UseSaveManager != value)
                {
                    Model.UseSaveManager = value;
                    _databaseService.SaveChanges();
                    OnPropertyChanged(nameof(UseSaveManager));
                }
            }
        }

        public bool UseDebugMode
        {
            get => Model.UseDebugMode;
            set
            {
                if (Model.UseDebugMode != value)
                {
                    Model.UseDebugMode = value;
                    _databaseService.SaveChanges();
                    OnPropertyChanged(nameof(UseDebugMode));
                }
            }
        }

        public bool UseScyllaHide
        {
            get => Model.UseScyllaHide;
            set
            {
                if(Model.UseScyllaHide != value)
                {
                    Model.UseScyllaHide = value;
                    _databaseService.SaveChanges();
                    OnPropertyChanged(nameof(UseScyllaHide));
                }
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

            if(Model.Mods is null)
            {
                Mods = new ObservableCollection<ModVm>();
            }
            else
            {
                Mods = new ObservableCollection<ModVm>(
                    Model.Mods.Select(x => new ModVm(x, _databaseService)));
            }

            if (Model.Dlls is null)
            {
                ExternalDlls = new ObservableCollection<DllVm>();
            }
            else
            {
                ExternalDlls = new ObservableCollection<DllVm>(
                    Model.Dlls.Select(x => new DllVm(x, _databaseService)));
            }
        }

        public async Task RefreshAsync()
        {
            Model = _databaseService
                .GetProfiles()
                .Single(x => new ProfileEqualityComparer().Equals(x, Model));

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(ImagePath));
            OnPropertyChanged(nameof(Created));
            OnPropertyChanged(nameof(LastPlayed));
            OnPropertyChanged(nameof(UseSaveManager));
            OnPropertyChanged(nameof(UseDebugMode));
            OnPropertyChanged(nameof(UseScyllaHide));

            var modVms = Model.Mods?.Select(x => new ModVm(x, _databaseService));
            var dllVms = Model.Dlls?.Select(x => new DllVm(x, _databaseService));

            await _dispatcherService.InvokeUiAsync(() =>
            {
                Mods.Clear();
                if (modVms is not null)
                {
                    foreach (var modVm in modVms)
                    {
                        Mods.Add(modVm);
                    }
                }                

                ExternalDlls.Clear();
                if(dllVms is not null)
                {
                    foreach (var dllVm in dllVms)
                    {
                        ExternalDlls.Add(dllVm);
                    }
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
