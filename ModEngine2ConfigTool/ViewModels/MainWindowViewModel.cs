using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableObject _currentContent;

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
            _currentContent = new FrontPageViewModel();

            ConfigureProfilesCommand = new RelayCommand(ConfigureProfiles);
            ConfigureSettingsCommand = new RelayCommand(ConfigureSettings);
            OpenLicencesCommand = new RelayCommand(OpenLicences);
        }

        private void ConfigureProfiles()
        {
            CurrentContent = new FrontPageViewModel();
        }

        private void ConfigureSettings()
        {
            CurrentContent = new SettingsViewModel();
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
