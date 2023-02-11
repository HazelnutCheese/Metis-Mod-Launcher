using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ModEngine2ConfigTool.Models
{

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
