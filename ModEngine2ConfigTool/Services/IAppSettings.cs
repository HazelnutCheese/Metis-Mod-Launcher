namespace ModEngine2ConfigTool.Services
{
    public interface IAppSettings
    {
        string EldenRingGameFolder { get; set; }
        string ModEngine2Folder { get; set; }
        string SaveGameFolder { get; set;}
    }
}
