using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModEngine2ConfigTool.ViewModels
{
    public class ExternalDllListViewModel : BaseOnDiskListViewModel<ExternalDllViewModel>
    {
        private string _lastOpenedLocation;

        public ExternalDllListViewModel(List<ExternalDllViewModel> modList) : base(modList)
        {
            _lastOpenedLocation = string.Empty;
        }

        protected override void AddNew()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Dll files (*.dll)|*.dll|All files (*.*)|*.*",
                Multiselect = true,
                Title = "Select Dll/s",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (_lastOpenedLocation.Equals(string.Empty))
            {
                fileDialog.InitialDirectory = OnDiskObjectList.Any()
                    ? Path.GetDirectoryName(OnDiskObjectList.First().Location)
                    : Directory.GetCurrentDirectory();
            }
            else
            {
                fileDialog.InitialDirectory = _lastOpenedLocation;
            }

            if(fileDialog.ShowDialog().Equals(true))
            {
                foreach (var file in fileDialog.FileNames)
                {
                    if(OnDiskObjectList.Any(x => x.Location == file) || !File.Exists(file))
                    {
                        return;
                    }

                    _lastOpenedLocation = Path.GetDirectoryName(file) ?? string.Empty;

                    var newMod = new ExternalDllViewModel(file);
                    OnDiskObjectList.Add(newMod);
                }
            }
        }
    }
}
