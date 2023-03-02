using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services.Interfaces;
using System;
using System.IO;

namespace ModEngine2ConfigTool.ViewModels.ProfileComponents
{
    public sealed class ModVm : ObservableObject
    {
        private string _folderPath;
        private string _name;
        private string _description;
        private string _imagePath;
        private readonly IDatabaseService _databaseService;

        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _name, value);
                    Model.Name = value;
                    _databaseService.SaveChanges();
                }
            }
        }

        public string FolderPath
        { 
            get => _folderPath;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _folderPath, value);
                    Model.FolderPath = value;
                    _databaseService.SaveChanges();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _description, value);
                    Model.Description = value;
                    _databaseService.SaveChanges();
                }
            }
        }
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _imagePath, value);
                    Model.ImagePath = value;
                    _databaseService.SaveChanges();
                }
            }
        }

        public DateTime Added => Model.Added;

        public Mod Model { get; }

        public ModVm(
            Mod mod, 
            IDatabaseService databaseService)
        {
            Model = mod;
            _databaseService = databaseService;

            _name = mod.Name;
            _folderPath = mod.FolderPath ?? "";
            _description = mod.Description ?? "";
            _imagePath = mod.ImagePath ?? "";
        }

        public ModVm(
            string folderPath, 
            IDatabaseService databaseService)
        {
            Model = new Mod
            {
                ModId = Guid.NewGuid(),
                Added = DateTime.Now,
                Name = new DirectoryInfo(folderPath).Name,
                FolderPath = Path.GetFullPath(folderPath),
                Description = string.Empty,
                ImagePath = string.Empty
            };
            _databaseService = databaseService;

            _name = Model.Name;
            _folderPath = Model.FolderPath ?? "";
            _description = Model.Description ?? "";
            _imagePath = Model.ImagePath ?? "";
        }
    }
}
