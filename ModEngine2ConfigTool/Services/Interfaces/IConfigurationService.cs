namespace ModEngine2ConfigTool.Services.Interfaces
{
    public interface IConfigurationService
    {
        bool? AutoDetectEldenRing { get; set; }
        bool? AutoDetectModEngine2 { get; set; }
        string EldenRingExePath { get; set; }
        string ModEngine2ExePath { get; set; }
    }
}