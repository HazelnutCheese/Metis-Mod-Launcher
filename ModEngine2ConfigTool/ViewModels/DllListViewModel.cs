using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModEngine2ConfigTool.ViewModels
{
    public class DllListViewModel : ModListViewModel
    {
        private string _lastOpenedLocation;

        public DllListViewModel(string header, IEnumerable<ModViewModel> modList) : base(header, modList)
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
                fileDialog.InitialDirectory = ProfileModsList.Any()
                    ? Path.GetDirectoryName(ProfileModsList.First().Location)
                    : Directory.GetCurrentDirectory();
            }
            else
            {
                fileDialog.InitialDirectory = _lastOpenedLocation;
            }

            if(fileDialog.ShowDialog().Equals(true))
            {
                foreach(var file in fileDialog.FileNames)
                {
                    var dllName = new FileInfo(file).Name;

                    var newMod = new ModViewModel(dllName, file);

                    if(ProfileModsList.Any(x => x.Name== dllName && x.Location == file))
                    {
                        return;
                    }

                    ProfileModsList.Add(newMod);
                }
            }
        }
    }
}
