using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.ViewModels.Fields;
using System.Collections.Generic;
using System.Linq;

namespace ModEngine2ConfigTool.ViewModels
{

    public class ProfileViewModel : ObservableObject, IIsChanged
    {        
        private string _name;

        public string OriginalName { get; }

        public string Name
        {
            get => _name;
            set 
            {
                SetProperty(ref _name, value);
                OnPropertyChanged(nameof(IIsChanged.IsChanged));
            }
        }

        public FieldsCollectionViewModel Fields { get; }

        public ModFolderListViewModel ModFolderListViewModel { get; set; }

        public ExternalDllListViewModel DllListViewModel { get; set; }

        public bool IsChanged =>
            NameIsChanged ||
            Fields.IsChanged || 
            ModFolderListViewModel.IsChanged || 
            DllListViewModel.IsChanged;

        public bool NameIsChanged => Name != OriginalName;

        public ProfileViewModel(ProfileModel profileModel)
        {
            OriginalName = profileModel.Name;
            _name = profileModel.Name;

            ModFolderListViewModel = new ModFolderListViewModel(
                profileModel.Mods.Select(x => new ModViewModel(x)).ToList());

            DllListViewModel = new ExternalDllListViewModel(
                profileModel.Dlls.Select(x => new ExternalDllViewModel(x.Location)).ToList());

            Fields = new FieldsCollectionViewModel(new List<IFieldViewModel>
            {
                new BoolFieldViewModel("Enable ME2 Debug Mode:", "", profileModel.EnableME2Debug),
                new BoolFieldViewModel("Ignore Mod Folders:", "", profileModel.IgnoreModFolders),
                new BoolFieldViewModel("Enable ScyllaHide Extension:", "", profileModel.EnableScyllaHide)
            });

            ModFolderListViewModel.PropertyChanged += Children_PropertyChanged;
            DllListViewModel.PropertyChanged += Children_PropertyChanged;
            Fields.PropertyChanged += Children_PropertyChanged;
        }

        private void Children_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Equals(e.PropertyName, nameof(IIsChanged.IsChanged)))
            {
                OnPropertyChanged(nameof(IsChanged));
            }
        }
    }
}
