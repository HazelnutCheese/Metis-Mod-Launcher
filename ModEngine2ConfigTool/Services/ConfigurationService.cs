using Microsoft.Extensions.Configuration;

namespace ModEngine2ConfigTool.Services
{
    public class ConfigurationService
    {
        readonly IConfigurationRoot _config;

        public string EldenRingGameFolder
        {
            get => _config["Settings:EldenRingGameFolder"] ?? string.Empty; 
            set => _config["Settings:EldenRingGameFolder"] = value;
        }

        public string ModEngine2Folder
        {
            get => _config["Settings:ModEngine2Folder"] ?? string.Empty;
            set => _config["Settings:ModEngine2Folder"] = value;
        }

        public ConfigurationService() 
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json");

            _config = configuration.Build();
        }
    }
}
