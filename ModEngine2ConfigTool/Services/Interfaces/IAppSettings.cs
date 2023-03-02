namespace ModEngine2ConfigTool.Services.Interfaces
{
    public interface IAppSettings
    {
        bool? AutoDetectModEngine2 { get; set; }

        string ModEngine2ExePath { get; set; }

        bool? AutoDetectEldenRing { get; set; }

        string EldenRingExePath { get; set; }
    }
}
