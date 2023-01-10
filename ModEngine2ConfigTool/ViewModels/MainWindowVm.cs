using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using ModEngine2ConfigTool.Extensions;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Dialogs;
using ModEngine2ConfigTool.ViewModels.Fields;
using ModEngine2ConfigTool.Views.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainWindowVm : ObservableObject
    {
        private ObservableObject _currentContent;
        private FrontPageViewModel _frontPageViewModel;
        private SettingsViewModel _settingsViewModel;

        public MainHostVm MainHostVm { get; }

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

        public ICommand ShowHelpCommand { get; }

        public MainWindowVm(MainWindow mainWindow)
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

            ShowHelpCommand = new AsyncRelayCommand(ShowHelp);

            _currentContent = _settingsViewModel.HasErrors 
                ? _settingsViewModel 
                : _frontPageViewModel;

            MainHostVm = new MainHostVm(mainWindow);
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

        private async Task ShowHelp()
        {
            var helpText = 
                "This tool is designed to help manage different modding setups for ModEngine2." +
                "\n\n" +
                "On first launch you will be taken to the Setting page. Here you will need to define " +
                "the paths to you Elden Ring install \"Game\" folder and to the root folder of ModEngine2." +
                " Once these have been set you must save your changes and click the \"Configure and Play\"" +
                "button on the top bar." +
                "\n\n" +
                "Once you are in the profile screen you will be able to create new modding profiles. The " +
                "avaliable options are described below:" +
                "\n\n" +
                "Enable ME2 Debug Mode: If set to true the ME2 debug console will appear while the game is running" +
                "\n\n" +
                "Ignore Mod Folders: If set to true this will prevent loading any mod files from the Mod Folders" +
                " section. When launching with a profile, External Dlls and config options will still be used." +
                "\n\n" +
                "Enable Scylla Hide Extension: Injects Scylla Hide into the game, bypassing antidebug measures." +
                " This option is for users who are attempting to inject Debuggers such as CheatEngine, x65dbg," +
                " windbg, etc. You are unlikely to need this option if you are not reverse engineering." +
                "\n\n" +
                "Mod Folders: Choose the folders which contain the mod files you want to load. These folders " +
                "contain files such as Regulation.bin and any \"maps\" or \"msg\" folders." +
                "\n\n" +
                "External Dlls: Choose any external dlls you want to load. For instance if you want to use " +
                "Seamless Coop you should add the Seamless Coop dll to the list." +
                "\n\n" +
                "More help is available on the Github page.";

            var openGithubPageCommand = new RelayCommand(() =>
            {
                Process.Start("explorer", "https://github.com/HazelnutCheese/ModEngine2ConfigTool");
            });

            var dialog = new CustomDialogView();
            var dialogVm = new CustomDialogViewModel(
                "Help",
                helpText,
                new List<IFieldViewModel>(),
                new List<DialogButtonViewModel>()
                {
                    new DialogButtonViewModel("View Github Page", openGithubPageCommand, true),
                    new DialogButtonViewModel("Close", CustomDialogViewModel.GetCloseDialogCommand(false, dialog), true)
                });

            dialog.DataContext = dialogVm;

            await DialogHost.Show(dialog, App.DialogHostId);
        }

        private void OpenLicences()
        {
            using var modEngine2 = Process.Start(
                "explorer",
                "https://github.com/soulsmods/ModEngine2/blob/main/LICENSE-MIT");

            using var communityToolkit = Process.Start(
                "explorer",
                "https://github.com/CommunityToolkit/WindowsCommunityToolkit/blob/main/License.md");

            using var materialDesignInXAML = Process.Start(
                "explorer", 
                "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/master/LICENSE");

            using var tommy = Process.Start(
                "explorer",
                "https://github.com/dezhidki/Tommy/blob/master/LICENSE");

            using var folderBrowserEx = Process.Start(
                "explorer",
                "https://github.com/evaristocuesta/FolderBrowserEx/blob/master/LICENSE");

            using var sherlog = Process.Start(
                "explorer",
                "https://github.com/sschmid/Sherlog/blob/main/LICENSE.md");
        }
    }
}
