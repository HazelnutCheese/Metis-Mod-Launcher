using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
