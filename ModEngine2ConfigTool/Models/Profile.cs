using System;
using System.Collections.Generic;

namespace ModEngine2ConfigTool.Models
{
    public sealed class Profile
    {
        public Guid ProfileId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastPlayed { get; set; }
        public List<Mod> Mods { get; set; }
        public List<Dll> Dlls { get; set; }

        public string ModsOrder { get; set; }

        public string DllsOrder { get; set; }

        public bool UseSaveManager { get; set; }

        public bool UseDebugMode { get; set; } = false;

        public bool UseScyllaHide { get; set; } = false;
    }
}
