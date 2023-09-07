using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using System.Collections.ObjectModel;
using System;
using System.Windows.Input;
using System.IO;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    internal class SettingsPageVm : ObservableObject
    {
        private readonly ConfigurationService _configurationService;
        private readonly DialogService _dialogService;

        public HotBarVm HotBarVm { get; }
        public string BackgroundImage { get; }

        public bool? AutoDetectModEngine2 
        { 
            get => _configurationService.AutoDetectModEngine2; 
            set
            {
                _configurationService.AutoDetectModEngine2 = value;
                OnPropertyChanged(nameof(AutoDetectModEngine2));
                (BrowseModEngine2Command as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public string ModEngine2ExePath 
        {
            get => _configurationService.ModEngine2ExePath;
            set
            {
                if(System.IO.File.Exists(value))
                {
                    _configurationService.ModEngine2ExePath = value;
                    OnPropertyChanged(nameof(ModEngine2ExePath));
                }
            }
        }

        public bool? AutoDetectEldenRing 
        {
            get => _configurationService.AutoDetectEldenRing;
            set
            {
                _configurationService.AutoDetectEldenRing = value;
                OnPropertyChanged(nameof(AutoDetectEldenRing));
                (BrowseEldenRingExeCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public string EldenRingExePath 
        {
            get => _configurationService.EldenRingExePath;
            set
            {
                if (System.IO.File.Exists(value))
                {
                    _configurationService.EldenRingExePath = value;
                    OnPropertyChanged(nameof(EldenRingExePath));
                }
            }
        }

        public bool? AutoDetectSaves
        {
            get => _configurationService.AutoDetectSaves;
            set
            {
                _configurationService.AutoDetectSaves = value;
                OnPropertyChanged(nameof(AutoDetectSaves));
                (BrowseEldenRingExeCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }

        public string EldenRingSavesPath
        {
            get => _configurationService.EldenRingSavesPath;
            set
            {
                if (System.IO.Directory.Exists(value))
                {
                    _configurationService.EldenRingSavesPath = value;
                    OnPropertyChanged(nameof(EldenRingSavesPath));
                }
            }
        }

        public ICommand BrowseEldenRingExeCommand { get; }

        public ICommand BrowseModEngine2Command { get; }

        public ICommand BrowseSavesCommand { get; }

        public SettingsPageVm(
            ConfigurationService configurationService,
            DialogService dialogService)
        {
            _configurationService = configurationService;
            _dialogService = dialogService;

            BrowseEldenRingExeCommand = new RelayCommand(BrowseEldenRing, () => AutoDetectEldenRing is false);
            BrowseModEngine2Command = new RelayCommand(BrowseModEngine2, () => AutoDetectModEngine2 is false);
            BrowseSavesCommand = new RelayCommand(BrowseSaves, () => AutoDetectSaves is false);

            HotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    //new HotBarButtonVm(
                    //    "Create new Profile",
                    //    PackIconKind.PencilOutline,
                    //    async () => await NavigateToCreateProfileAsync())
                });

            BackgroundImage = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Background_05.png");
        }

        private void BrowseEldenRing()
        {
            var newPath = _dialogService.ShowOpenFileDialog(
                "Select Elden Ring Exe",
                "Exe files (*.exe)|*.exe",
                EldenRingExePath,
                EldenRingExePath);

            if(newPath is string && System.IO.File.Exists(newPath))
            {
                EldenRingExePath = newPath;
            }
        }

        private void BrowseModEngine2()
        {
            var newPath = _dialogService.ShowOpenFileDialog(
                "Select ModEngine2 Exe",
                "Exe files (*.exe)|*.exe",
                ModEngine2ExePath,
                ModEngine2ExePath);

            if (newPath is string && System.IO.File.Exists(newPath))
            {
                ModEngine2ExePath = newPath;
            }
        }

        private void BrowseSaves()
        {
            var newPath = _dialogService.ShowFolderDialog(
                "Select Elden Ring Saves folder",
                EldenRingSavesPath,
                EldenRingSavesPath);

            if (newPath is string && System.IO.Directory.Exists(newPath))
            {
                EldenRingSavesPath = newPath;
            }
        }
    }
}
