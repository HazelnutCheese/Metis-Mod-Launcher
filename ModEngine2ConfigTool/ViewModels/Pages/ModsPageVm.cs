﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.Views.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    internal class ModsPageVm : ObservableObject
    {
        private ICollectionView _mods;
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly ModManagerService _modManagerService;

        private readonly ObservableCollection<ModListButtonVm> _modListButtons;

        public ICollectionView Mods
        {
            get => _mods;
            set => SetProperty(ref _mods, value);
        }

        public ICommand SortByNameCommand { get; }

        public ICommand SortByPathCommand { get; }

        public ICommand SortByDescriptionCommand { get; }

        public ICommand SortByDateAddedCommand { get; }

        public HotBarVm HotBarVm { get; }

        public ModsPageVm(
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            ModManagerService modManagerService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _modManagerService = modManagerService;

            _modListButtons = new ObservableCollection<ModListButtonVm>();
            UpdateModListButtons();

            _mods = CollectionViewSource.GetDefaultView(_modListButtons);
            _mods.SortDescriptions.Add(new SortDescription(nameof(ModListButtonVm.Name), ListSortDirection.Descending));

            modManagerService.ModVms.CollectionChanged += _baseMods_CollectionChanged;

            SortByNameCommand = new AsyncRelayCommand<SortButtonMode>(SortByName);
            SortByDescriptionCommand = new AsyncRelayCommand<SortButtonMode>(SortByDescription);
            SortByPathCommand = new AsyncRelayCommand<SortButtonMode>(SortByPath);
            SortByDateAddedCommand = new AsyncRelayCommand<SortButtonMode>(SortByDateAdded);

            HotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    new HotBarButtonVm(
                        "Import Mod",
                        PackIconKind.FolderMultiplePlusOutline,
                        async () => await NavigateToImportModAsync())
                });
        }

        private void _baseMods_CollectionChanged(
            object? sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateModListButtons();
            _mods.Refresh();
        }

        private async Task SortByName(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if(sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModListButtonVm.Name), 
                        ListSortDirection.Descending));
                }
                else if(sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModListButtonVm.Name), 
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortByDescription(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModListButtonVm.Description),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModListButtonVm.Description),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortByPath(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModListButtonVm.FolderPath),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModListButtonVm.FolderPath),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortByDateAdded(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModListButtonVm.Added),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModListButtonVm.Added),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task NavigateToImportModAsync()
        {
            var modVm = await _modManagerService.ImportModAsync();
            if (modVm is null)
            {
                return;
            }

            var modEditPage = new ModEditPageVm(
                modVm,
                true,
                _navigationService,
                _profileManagerService,
                _modManagerService);

            await _navigationService.NavigateTo(modEditPage);
        }

        private void UpdateModListButtons()
        {
            _modListButtons.Clear();
            foreach(var mod in _modManagerService.ModVms)
            {
                _modListButtons.Add(new ModListButtonVm(
                    mod, 
                    _navigationService, 
                    _profileManagerService, 
                    _modManagerService));
            }
        }
    }
}
