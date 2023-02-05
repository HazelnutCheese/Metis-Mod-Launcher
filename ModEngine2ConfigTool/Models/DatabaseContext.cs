using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;

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
    }

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

    public sealed class Dll
    {
        public Guid DllId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public string? FilePath { get; set; }
        public DateTime Added { get; set; }
        public List<Profile> Profiles { get; set; }
    }

    public sealed class DatabaseContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Mod> Mods { get; set; }
        public DbSet<Dll> Dlls { get; set; }

        public string DbPath { get; }

        public DatabaseContext()
        {
            DbPath = Path.Join(Directory.GetCurrentDirectory(), "ModEngine2ConfigTool.db");
        }

        public DatabaseContext(string dataStorageFolder)
        {
            DbPath = Path.Join(dataStorageFolder, "ModEngine2ConfigTool.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
