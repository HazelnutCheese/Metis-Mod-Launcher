using System;
using System.Collections.Generic;

namespace ModEngine2ConfigTool.Models
{
    public sealed class Mod
    {
        public Guid ModId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public string? FolderPath { get; set; }
        public DateTime Added { get; set; }
        public List<Profile> Profiles { get; set; }
    }
}
