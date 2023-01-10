using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public ICollectionView Mods
        {
            get => _mods;
            set => SetProperty(ref _mods, value);
        }

        public ICommand SortByNameCommand { get; }

        public ICommand SortByPathCommand { get; }

        public ICommand SortByDescriptionCommand { get; }

        public ICommand SortByDateAddedCommand { get; }

        public ModsPageVm(ObservableCollection<ModVm> mods)
        {
            _mods = CollectionViewSource.GetDefaultView(mods);
            _mods.SortDescriptions.Add(new SortDescription(nameof(ModVm.Name), ListSortDirection.Descending));

            mods.CollectionChanged += _baseMods_CollectionChanged;

            SortByNameCommand = new AsyncRelayCommand<SortButtonMode>(SortByName);
            SortByDescriptionCommand = new AsyncRelayCommand<SortButtonMode>(SortByDescription);
            SortByPathCommand = new AsyncRelayCommand<SortButtonMode>(SortByPath);
            SortByDateAddedCommand = new AsyncRelayCommand<SortButtonMode>(SortByDateAdded);
        }

        private void _baseMods_CollectionChanged(
            object? sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
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
                        nameof(ModVm.Name), 
                        ListSortDirection.Descending));
                }
                else if(sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.Name), 
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
                        nameof(ModVm.Description),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.Description),
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
                        nameof(ModVm.FolderPath),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.FolderPath),
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
                        nameof(ModVm.Added),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _mods.SortDescriptions.Clear();
                    _mods.SortDescriptions.Add(new SortDescription(
                        nameof(ModVm.Added),
                        ListSortDirection.Ascending));
                }
            });
        }
    }
}
