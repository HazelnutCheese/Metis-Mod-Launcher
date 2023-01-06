using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Models;
using System.Linq;

namespace ModEngine2ConfigTool.ViewModels
{

    public class ProfileViewModel : ObservableObject
    {
        private bool _enableModLoaderConfiguration;
        private bool _enableME2Debug;
        private bool _enableScyllaHide;

        public string Name { get; set; }

        public bool EnableME2Debug
        {
            get => _enableME2Debug;
            set => SetProperty(ref _enableME2Debug, value);
        }

        public bool EnableModLoaderConfiguration 
        { 
            get => _enableModLoaderConfiguration; 
            set => SetProperty(ref _enableModLoaderConfiguration, value); 
        }

        public bool EnableScyllaHide
        {
            get => _enableScyllaHide;
            set => SetProperty(ref _enableScyllaHide, value);
        }

        public ModFolderListViewModel ModFolderListViewModel { get; set; }

        public DllListViewModel DllListViewModel { get; set; }

        public ProfileViewModel(ProfileModel profileModel)
        {
            Name = profileModel.Name;

            EnableME2Debug = profileModel.EnableME2Debug;
            EnableModLoaderConfiguration = profileModel.EnableModLoaderConfiguration;
            EnableScyllaHide= profileModel.EnableScyllaHide;

            ModFolderListViewModel = new ModFolderListViewModel(
                profileModel.Mods.Select(x => new ModViewModel(x)));

            DllListViewModel = new DllListViewModel(
                profileModel.Dlls.Select(x => new ModViewModel(x)));
        }
    }
}
