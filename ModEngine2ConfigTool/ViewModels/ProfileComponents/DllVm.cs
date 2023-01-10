using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;

namespace ModEngine2ConfigTool.ViewModels.ProfileComponents
{
    public class DllVm : ObservableObject
    {
        private string _filePath;
        private string _name;
        private string _description;
        private string _imagePath;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
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

        public DllVm(string filePath)
        {
            _name = Path.GetFileNameWithoutExtension(filePath);
            _filePath = Path.GetFullPath(filePath);
            _description = string.Empty;
            _imagePath = string.Empty;
            Added = DateTime.Now;
        }

        public DllVm(
            string name,
            string filePath,
            string description,
            string imagePath,
            DateTime added)
        {
            _name = name;
            _filePath = filePath;
            _description = description;
            _imagePath = imagePath;
            Added = added;
        }
    }
}
