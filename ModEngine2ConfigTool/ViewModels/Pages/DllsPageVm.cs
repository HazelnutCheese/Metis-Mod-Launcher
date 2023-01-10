using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.Views.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    internal class DllsPageVm : ObservableObject
    {
        private ICollectionView _dlls;

        public ICollectionView Dlls
        {
            get => _dlls;
            set => SetProperty(ref _dlls, value);
        }

        public ICommand SortByNameCommand { get; }

        public ICommand SortByPathCommand { get; }

        public ICommand SortByDescriptionCommand { get; }

        public ICommand SortByDateAddedCommand { get; }

        public DllsPageVm(ObservableCollection<DllVm> dlls)
        {            
            _dlls = CollectionViewSource.GetDefaultView(dlls);
            _dlls.SortDescriptions.Add(new SortDescription(
                nameof(DllVm.Name),
                ListSortDirection.Descending));

            dlls.CollectionChanged += _dlls_CollectionChanged;

            SortByNameCommand = new AsyncRelayCommand<SortButtonMode>(SortByName);
            SortByDescriptionCommand = new AsyncRelayCommand<SortButtonMode>(SortByDescription);
            SortByPathCommand = new AsyncRelayCommand<SortButtonMode>(SortByPath);
            SortByDateAddedCommand = new AsyncRelayCommand<SortButtonMode>(SortByDateAdded);
        }

        private void _dlls_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
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
                        nameof(DllVm.Name), 
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllVm.Name), 
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
                        nameof(DllVm.Description),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllVm.Description),
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
                        nameof(DllVm.FilePath),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllVm.FilePath),
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
                        nameof(DllVm.Added),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _dlls.SortDescriptions.Clear();
                    _dlls.SortDescriptions.Add(new SortDescription(
                        nameof(DllVm.Added),
                        ListSortDirection.Ascending));
                }
            });
        }
    }
}
