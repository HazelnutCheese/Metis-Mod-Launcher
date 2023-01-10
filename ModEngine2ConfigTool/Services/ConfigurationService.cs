using Config.Net;
using System.IO;

namespace ModEngine2ConfigTool.Services
{
    public class ConfigurationService
    {
        readonly IAppSettings _settings;

        [Option(Alias = "EldenRingGameFolder")]
        public string EldenRingGameFolder
        {
            get => _settings.EldenRingGameFolder; 
            set => _settings.EldenRingGameFolder = value;
        }

        [Option(Alias = "ModEngine2Folder")]
        public string ModEngine2Folder
        {
            get => _settings.ModEngine2Folder;
            set => _settings.ModEngine2Folder = value;
        }

        [Option(Alias = "SaveGameFolder")]
        public string SaveGameFolder
        {
            get => _settings.SaveGameFolder;
            set => _settings.SaveGameFolder = value;
        }

        public ConfigurationService() 
        {
            var configFile = Path.Combine(App.DataStorage, "appsettings.json");

            _settings = new ConfigurationBuilder<IAppSettings>()
               .UseJsonFile(configFile)
               .Build();
        }
    }
}
