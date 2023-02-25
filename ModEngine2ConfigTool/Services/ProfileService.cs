using ModEngine2ConfigTool.ViewModels.Profiles;
using System.IO;
using System.IO.Compression;
using Tommy;

namespace ModEngine2ConfigTool.Services
{
    public class ProfileService
    {
        private readonly string _rootProfilesFolder;

        public ProfileService(string dataStorage)
        {
            _rootProfilesFolder = Path.Combine(dataStorage, "temp");
            if(!Directory.Exists(_rootProfilesFolder))
            {
                Directory.CreateDirectory(_rootProfilesFolder);
            }            
        }

        public string WriteProfile(ProfileVm profile)
        {
            var fileName = GetProfilePath(profile.Model.ProfileId.ToString());

            using TextWriter writer = File.CreateText(fileName);

            var dllListToml = new TomlArray();
            foreach(var dll in profile.ExternalDlls)
            {
                dllListToml.Add(dll.FilePath);
            }

            var modsListToml = new TomlArray();
            foreach (var mod in profile.Mods)
            {
                modsListToml.Add(new TomlTable
                {
                    ["enabled"] = true,
                    ["name"] = mod.Model.ModId.ToString(),
                    ["path"] = mod.FolderPath
                });
            }

            var fileToml = new TomlTable
            {
                ["modengine"] =
                {
                    ["debug"] = profile.UseDebugMode,
                    ["external_dlls"] = dllListToml
                },
                ["extension"] =
                {
                    ["mod_loader"] =
                    {
                        ["enabled"] = true,
                        ["loose_params"] = false,
                        ["mods"] = modsListToml,
                    }
                },                
                ["extension"] =
                {
                    ["scylla_hide"] =
                    {
                        ["enabled"] = profile.UseScyllaHide
                    }
                },
            };

            fileToml.WriteTo(writer);
            writer.Flush();

            return fileName;
        }

        public string GetProfilePath(string profileId)
        {
            return Path.Combine(_rootProfilesFolder, $"{profileId}.toml");
        }        
    }
}
