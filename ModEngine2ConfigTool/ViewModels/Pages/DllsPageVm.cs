using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.Views.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    internal class DllsPageVm : ObservableObject
    {
        private ICollectionView _dlls;
        private readonly NavigationService _navigationService;
        private readonly ProfileManagerService _profileManagerService;
        private readonly DllManagerService _dllManagerService;

        private readonly ObservableCollection<DllListButtonVm> _dllListButtons;

        public ICollectionView Dlls
        {
            get => _dlls;
            set => SetProperty(ref _dlls, value);
        }

        public ICommand SortByNameCommand { get; }

        public ICommand SortByPathCommand { get; }

        public ICommand SortByDescriptionCommand { get; }

        public ICommand SortByDateAddedCommand { get; }

        public HotBarVm HotBarVm { get; }
        public string BackgroundImage { get; }

        public DllsPageVm(
            NavigationService navigationService,
            ProfileManagerService profileManagerService,
            DllManagerService dllManagerService)
        {
            _navigationService = navigationService;
            _profileManagerService = profileManagerService;
            _dllManagerService = dllManagerService;

            _dllListButtons = new ObservableCollection<DllListButtonVm>();
            UpdateDllListButtons();

            _dlls = CollectionViewSource.GetDefaultView(_dllListButtons);
            _dlls.SortDescriptions.Add(new SortDescription(nameof(DllListButtonVm.Name), ListSortDirection.Descending));

            _dllManagerService.DllVms.CollectionChanged += _dlls_CollectionChanged;

            SortByNameCommand = new AsyncRelayCommand<SortButtonMode>(SortByName);
            SortByDescriptionCommand = new AsyncRelayCommand<SortButtonMode>(SortByDescription);
            SortByPathCommand = new AsyncRelayCommand<SortButtonMode>(SortByPath);
            SortByDateAddedCommand = new AsyncRelayCommand<SortButtonMode>(SortByDateAdded);

            HotBarVm = new HotBarVm(
                new ObservableCollection<ObservableObject>()
                {
                    new HotBarButtonVm(
                        "Add from File",
                        PackIconKind.FilePlusOutline,
                        async () => await NavigateToImportDllAsync()),
                    //new HotBarButtonVm(
                    //    "Add from Package",
                    //    PackIconKind.PackageVariantClosedPlus,
                    //    async () => await NavigateToImportDllAsync())
                });

            BackgroundImage = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Background_03.png");
        }

        private void _dlls_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateDllListButtons();
            _dlls.Refresh();
        }

        public async Task SortByName(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllListButtonVm.Name), 
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllListButtonVm.Name), 
                        ListSortDirection.Ascending));
                }
            });
        }

        public async Task SortByDescription(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllListButtonVm.Description),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllListButtonVm.Description),
                        ListSortDirection.Ascending));
                }
            });
        }

        public async Task SortByPath(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllListButtonVm.FilePath),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllListButtonVm.FilePath),
                        ListSortDirection.Ascending));
                }
            });
        }

        public async Task SortByDateAdded(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllListButtonVm.Added),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllListButtonVm.Added),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task NavigateToImportDllAsync()
        {
            var dllVm = await _dllManagerService.ImportDllAsync();
            if (dllVm is null)
            {
                return;
            }

            await _navigationService.NavigateTo<DllEditPageVm>(
                new NamedParameter("dll", dllVm));
        }

        private void UpdateDllListButtons()
        {
            _dllListButtons.Clear();
            foreach (var dll in _dllManagerService.DllVms)
            {
                _dllListButtons.Add(new DllListButtonVm(
                    dll,
                    _navigationService,
                    _profileManagerService,
                    _dllManagerService));
            }
        }
    }
}
