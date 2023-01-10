using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using ModEngine2ConfigTool.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class ProfilesPageVm : ObservableObject
    {
        private ICollectionView _profiles;

        public ICollectionView Profiles
        {
            get => _profiles;
            set => SetProperty(ref _profiles, value);
        }

        public ICommand SortByNameCommand { get; }

        public ICommand SortByDescriptionCommand { get; }

        public ICommand SortByLastPlayedCommand { get; }

        public ICommand SortByCreatedCommand { get; }

        public ProfilesPageVm(ObservableCollection<ProfileVm> profiles)
        {
            _profiles = CollectionViewSource.GetDefaultView(profiles);
            _profiles.SortDescriptions.Add(new SortDescription(
                nameof(ProfileVm.Name), 
                ListSortDirection.Descending));

            profiles.CollectionChanged += Profiles_CollectionChanged;

            SortByNameCommand = new AsyncRelayCommand<SortButtonMode>(SortByName);
            SortByDescriptionCommand = new AsyncRelayCommand<SortButtonMode>(SortByDescription);
            SortByLastPlayedCommand = new AsyncRelayCommand<SortButtonMode>(SortByLastPlayed);
            SortByCreatedCommand = new AsyncRelayCommand<SortButtonMode>(SortByCreated);
        }

        private void Profiles_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _profiles.Refresh();
        }

        private async Task SortByName(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileVm.Name), 
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileVm.Name), 
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
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileVm.Description),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileVm.Description),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortByLastPlayed(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileVm.LastPlayed),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileVm.LastPlayed),
                        ListSortDirection.Ascending));
                }
            });
        }

        private async Task SortByCreated(SortButtonMode sortButtonMode)
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (sortButtonMode.Equals(SortButtonMode.Descending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileVm.Created),
                        ListSortDirection.Descending));
                }
                else if (sortButtonMode.Equals(SortButtonMode.Ascending))
                {
                    _profiles.SortDescriptions.Clear();
                    _profiles.SortDescriptions.Add(new SortDescription(
                        nameof(ProfileVm.Created),
                        ListSortDirection.Ascending));
                }
            });
        }
    }
}
