using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;

namespace ModEngine2ConfigTool.ViewModels.ProfileComponents
{
    public class ModVm : ObservableObject
    {
        private string _folderPath;
        private string _name;
        private string _description;
        private string _imagePath;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string FolderPath
        { 
            get => _folderPath; 
            set => SetProperty(ref _folderPath, value); 
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public DateTime Added { get; }

        public ModVm(string folderPath)
        {
            _name = new DirectoryInfo(folderPath).Name;
            _folderPath = Path.GetFullPath(folderPath);
            _description = string.Empty;
            _imagePath = string.Empty;
            Added = DateTime.Now;
        }

        public ModVm(
            string name,
            string folderPath,
            string description,
            string imagePath,
            DateTime added)
        {
            _name = name;
            _folderPath = folderPath;
            _description = description;
            _imagePath = imagePath;
            Added = added;
        }
    }
}
