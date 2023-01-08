using Config.Net;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace ModEngine2ConfigTool.Services
{
    public class ConfigurationService
    {
        readonly IAppSettings _settings;

        [Option(Alias = "Settings.EldenRingGameFolder")]
        public string EldenRingGameFolder
        {
            get => _settings.EldenRingGameFolder; 
            set => _settings.EldenRingGameFolder = value;
        }

        [Option(Alias = "Settings.ModEngine2Folder")]
        public string ModEngine2Folder
        {
            get => _settings.ModEngine2Folder;
            set => _settings.ModEngine2Folder = value;
        }

        public ConfigurationService() 
        {
            _settings = new ConfigurationBuilder<IAppSettings>()
               .UseJsonFile("appSettings.json")
               .Build();
        }
    }
}
