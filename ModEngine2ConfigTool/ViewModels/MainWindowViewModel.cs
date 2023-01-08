using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Extensions;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableObject _currentContent;
        private FrontPageViewModel _frontPageViewModel;
        private SettingsViewModel _settingsViewModel;

        public ObservableObject CurrentContent 
        { 
            get => _currentContent;
            set
            {
                SetProperty(ref _currentContent, value);
            }
        }

        public ICommand ConfigureProfilesCommand { get; }

        public ICommand ConfigureSettingsCommand { get; }

        public ICommand OpenLicencesCommand { get; }

        public MainWindowViewModel()
        {
            _frontPageViewModel = new FrontPageViewModel();
            _settingsViewModel = new SettingsViewModel();

            _settingsViewModel.PropertyChanged += _settingsViewModel_PropertyChanged;

            ConfigureProfilesCommand = new RelayCommand(
                ConfigureProfiles,
                () => !_settingsViewModel.HasErrors && !_settingsViewModel.Fields.IsChanged);

            ConfigureSettingsCommand = new RelayCommand(
                ConfigureSettings);

            OpenLicencesCommand = new RelayCommand(OpenLicences);

            _currentContent = _settingsViewModel.HasErrors 
                ? _settingsViewModel 
                : _frontPageViewModel;
        }

        private void _settingsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(Equals(e.PropertyName, nameof(SettingsViewModel.HasErrors)))
            {
                ConfigureProfilesCommand.NotifyCanExecuteChanged();
            }
        }

        private void ConfigureProfiles()
        {
            CurrentContent = _frontPageViewModel;
        }

        private void ConfigureSettings()
        {
            CurrentContent = _settingsViewModel;
        }

        private void OpenLicences()
        {
            using var communityToolkit = Process.Start(
                "explorer",
                "https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/License.md");

            using var materialDesignInXAML = Process.Start(
                "explorer", 
                "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/master/LICENSE");
        }
    }
}
