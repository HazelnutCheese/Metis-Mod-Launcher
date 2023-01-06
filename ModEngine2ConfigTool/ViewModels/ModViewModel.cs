using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Models;

namespace ModEngine2ConfigTool.ViewModels
{
    public class ModViewModel : ObservableObject
    {
        private string _name;
        private string _location;
        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled; 
            set => SetProperty(ref _isEnabled, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public ModViewModel(string name, string location)
        {
            _name = name;
            _location = location;
            _isEnabled = true;
        }

        public ModViewModel(ModModel modModel) 
        { 
            _name = modModel.Name;
            _location = modModel.Location;
            _isEnabled = modModel.IsEnabled;
        }
    }
}
