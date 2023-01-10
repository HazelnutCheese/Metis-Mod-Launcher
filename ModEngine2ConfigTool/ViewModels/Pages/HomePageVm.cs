using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class HomePageVm : ObservableObject
    {
        private readonly ObservableCollection<ProfileVm> _profiles;
        private ICollectionView _recentProfiles;

        public ICollectionView RecentProfiles 
        { 
            get => _recentProfiles;
            private set => SetProperty(ref _recentProfiles, value);
        }

        public HomePageVm(ObservableCollection<ProfileVm> profiles)
        {
            _profiles = profiles;

            _recentProfiles = CollectionViewSource.GetDefaultView(_profiles
                .OrderByDescending(x => x.LastPlayed)
                .Take(10));

            _profiles.CollectionChanged += async (s,e) => await UpdateRecentProfiles();
        }

        private async Task UpdateRecentProfiles()
        {
            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                RecentProfiles = CollectionViewSource.GetDefaultView(_profiles
                    .OrderByDescending(x => x.LastPlayed)
                    .Take(10));
            });
        }
    }
}
