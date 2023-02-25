using PowerArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool
{
    public class CommandLineArgs
    {
        [ArgShortcut("-p")]
        public string? ProfileId { get; set; }

        [ArgShortcut("-c")]
        public string? AppSettings { get; set; }

        [ArgShortcut("-l")]
        public string? AppData { get; set; }
    }
}
