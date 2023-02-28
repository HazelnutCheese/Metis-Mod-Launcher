using Config.Net;

namespace ModEngine2ConfigTool.Services
{
    public class ConfigurationService
    {
        readonly IAppSettings _settings;

        [Option(DefaultValue = true)]
        public bool? AutoDetectModEngine2
        {
            get => _settings.AutoDetectModEngine2;
            set => _settings.AutoDetectModEngine2 = value;
        }

        [Option(Alias = nameof(ModEngine2ExePath))]
        public string ModEngine2ExePath
        {
            get => _settings.ModEngine2ExePath;
            set => _settings.ModEngine2ExePath = value;
        }

        [Option(DefaultValue = true)]
        public bool? AutoDetectEldenRing
        {
            get => _settings.AutoDetectEldenRing;
            set => _settings.AutoDetectEldenRing = value;
        }

        [Option(Alias = nameof(EldenRingExePath))]
        public string EldenRingExePath
        {
            get => _settings.EldenRingExePath;
            set => _settings.EldenRingExePath = value;
        }

        public ConfigurationService(string configPath) 
        {
            var configFile = configPath;

            _settings = new ConfigurationBuilder<IAppSettings>()
               .UseJsonFile(configFile)
               .Build();

            _settings.AutoDetectEldenRing = _settings.AutoDetectEldenRing ?? true;
            _settings.AutoDetectModEngine2 = _settings.AutoDetectModEngine2 ?? true;
        }
    }
}
