using Config.Net;
using System.IO;

namespace ModEngine2ConfigTool.Services
{
    public class ConfigurationService
    {
        readonly IAppSettings _settings;

        [Option(Alias = "ModEngine2Folder")]
        public string ModEngine2Folder
        {
            get => _settings.ModEngine2Folder;
            set => _settings.ModEngine2Folder = value;
        }

        public ConfigurationService(string configPath) 
        {
            var configFile = configPath;

            _settings = new ConfigurationBuilder<IAppSettings>()
               .UseJsonFile(configFile)
               .Build();
        }
    }
}
