using PowerArgs;

namespace ModEngine2ConfigTool
{
    public class CommandLineArgs
    {
        [ArgShortcut("-p")]
        public string? ProfileId { get; set; }

        [ArgShortcut("-d")]
        public string? AppData { get; set; }
    }
}
