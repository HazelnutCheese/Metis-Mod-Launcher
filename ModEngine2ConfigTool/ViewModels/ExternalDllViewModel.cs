using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace ModEngine2ConfigTool.ViewModels
{
    public class ExternalDllViewModel : ObservableObject, IOnDiskViewModel
    {
        public string Name { get; }

        public string Location { get; }

        public ExternalDllViewModel(string location)
        {

            Name = Path.GetFileName(location);
            Location = location;
        }
    }
}
