using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.Services.Interfaces;
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

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _filePath, value);
                    Model.FilePath = value;
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

        public Dll Model { get; }

        public DllVm(
            string filePath,
            IDatabaseService databaseService)
        {
            Model = new Dll
            {
                DllId = Guid.NewGuid(),
                Added = DateTime.Now,
                Name = new FileInfo(filePath).Name,
                FilePath = Path.GetFullPath(filePath),
                Description = string.Empty,
                ImagePath = string.Empty
            };
            _databaseService = databaseService;

            _name = Model.Name;
            _filePath = Model.FilePath ?? "";
            _description = Model.Description ?? "";
            _imagePath = Model.ImagePath ?? "";
        }

        public DllVm(
            Dll dll,
            IDatabaseService databaseService)
        {
            Model = dll;
            _databaseService = databaseService;

            _name = dll.Name;
            _filePath = dll.FilePath ?? "";
            _description = dll.Description ?? "";
            _imagePath = dll.ImagePath ?? "";
        }
    }
}
