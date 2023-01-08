using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.ViewModels.Fields;

namespace ModEngine2ConfigTool.ViewModels
{

    public class ModViewModel : ObservableObject, IOnDiskViewModel, IIsChanged
    {
        private string _name;
        private string _location;
        private bool _isEnabled;

        private readonly string _originalName;
        private readonly bool _originalIsEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                SetProperty(ref _isEnabled, value);
                OnPropertyChanged(nameof(IsChanged));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                OnPropertyChanged(nameof(IsChanged));
            }
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public bool IsChanged => !Equals(Name, _originalName) 
            || !Equals(IsEnabled, _originalIsEnabled);

        public ModViewModel(string name, string location)
        {
            _originalName = name;
            _name = name;

            _location = location;

            _originalIsEnabled = true;
            _isEnabled = true;
        }

        public ModViewModel(ModModel modModel) 
        {
            _originalName = modModel.Name;
            _name = modModel.Name;

            _location = modModel.Location;

            _originalIsEnabled = modModel.IsEnabled;
            _isEnabled = modModel.IsEnabled;
        }
    }
}
