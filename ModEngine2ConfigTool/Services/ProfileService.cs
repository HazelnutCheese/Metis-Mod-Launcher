using Microsoft.Extensions.Configuration;
using ModEngine2ConfigTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tommy;
using Tommy.Extensions.Configuration;

namespace ModEngine2ConfigTool.Services
{
    public static class ProfileService
    {
        public static ProfileModel ReadProfile(string filePath) 
        {
            var profileName = Path.GetFileNameWithoutExtension(filePath);

            using StreamReader reader = File.OpenText(filePath);
            TomlTable table = TOML.Parse(reader);

            var enableModEngineDebug = (bool) table["modengine"]["debug"];
            var enableModLoaderConfiguration = (bool)table["extension"]["mod_loader"]["enabled"];
            var modFolders = new List<ModModel>();

            foreach (TomlNode node in table["extension"]["mod_loader"]["mods"])
            {
                var enabled = node["enabled"];
                var name = node["name"];
                var path = node["path"];

                modFolders.Add(new ModModel(name, path, enabled));
            }

            var enableScyllaHide = (bool)table["extension"]["scylla_hide"]["enabled"];

            return new ProfileModel(
                profileName,
                modFolders,
                new List<ModModel>(),
                enableModEngineDebug,
                enableModLoaderConfiguration,
                enableScyllaHide);
        }
    }
}
