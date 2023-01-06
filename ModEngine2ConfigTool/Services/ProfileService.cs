using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using Tommy;

namespace ModEngine2ConfigTool.Services
{
    public static class ProfileService
    {
        public static List<ProfileViewModel> LoadProfiles(string directory)
        {
            var files = Directory.GetFiles(directory, "*.toml");
            var results = new List<ProfileViewModel>();

            foreach(var tomlFile in files)
            {
                try
                {
                    var profile = ReadProfile(tomlFile);
                    results.Add(new ProfileViewModel(profile));
                }
                catch(Exception ex)
                {
                    App.Logger.Error(ex.ToString());
                }
            }

            return results;
        }

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

        public static void WriteProfile(ProfileModel profile)
        {
            var fileName = $".\\Profiles\\{profile.Name}_{Guid.NewGuid()}.toml";

            using TextWriter writer = File.CreateText(fileName);

            var modsListToml = new TomlArray();

            foreach (var mod in profile.Mods)
            {
                modsListToml.Add(new TomlTable
                {
                    ["isEnabled"] = mod.IsEnabled,
                    ["name"] = mod.Name,
                    ["path"] = mod.Location
                });
            }

            var fileToml = new TomlTable
            {
                ["modengine"] =
                {
                    ["debug"] = profile.EnableME2Debug
                },
                ["extension"] =
                {
                    ["mod_loader"] =
                    {
                        ["enabled"] = profile.EnableModLoaderConfiguration
                    }
                },
                ["loose_params"] = false,
                ["mods"] = modsListToml,
                ["extension"] =
                {
                    ["scylla_hide"] =
                    {
                        ["enabled"] = profile.EnableScyllaHide
                    }
                },
            };

            fileToml.WriteTo(writer);
            writer.Flush();
        }
    }
}
