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
        public static void Initialise()
        {
            if(!Directory.Exists(".\\Profiles"))
            {
                Directory.CreateDirectory(".\\Profiles");
            }
        }

        public static List<ProfileViewModel> LoadProfiles(string directory)
        {
            if(!Directory.Exists(directory))
            {
                return new List<ProfileViewModel>();
            }

            var files = Directory.GetFiles(directory, "*.toml");
            var results = new List<ProfileViewModel>();

            foreach(var tomlFile in files)
            {
                try
                {
                    var profile = ReadProfileFromFile(tomlFile);
                    results.Add(new ProfileViewModel(profile));
                }
                catch(Exception ex)
                {
                    App.Logger.Error(ex.ToString());
                }
            }

            return results;
        }

        public static ProfileModel ReadProfile(string profileName)
        {
            return ReadProfileFromFile($".\\Profiles\\{profileName}.toml");
        }

        public static void DeleteProfile(string profileName) 
        {
            File.Delete($".\\Profiles\\{profileName}.toml");
        }

        public static void RenameProfile(string currentName, string newName)
        {
            var currentPath = GetProfilePath(currentName);
            var newPath = GetProfilePath(newName);

            File.Move(currentPath, newPath, true);
        }

        public static void WriteProfile(ProfileModel profile)
        {
            var fileName = GetProfilePath(profile.Name);

            using TextWriter writer = File.CreateText(fileName);

            var dllListToml = new TomlArray();
            foreach(var dll in profile.Dlls)
            {
                dllListToml.Add(dll.Location);
            }

            var modsListToml = new TomlArray();
            foreach (var mod in profile.Mods)
            {
                modsListToml.Add(new TomlTable
                {
                    ["enabled"] = mod.IsEnabled,
                    ["name"] = mod.Name,
                    ["path"] = mod.Location
                });
            }

            var fileToml = new TomlTable
            {
                ["modengine"] =
                {
                    ["debug"] = profile.EnableME2Debug,
                    ["external_dlls"] = dllListToml
                },
                ["extension"] =
                {
                    ["mod_loader"] =
                    {
                        ["enabled"] = !profile.IgnoreModFolders,
                        ["loose_params"] = false,
                        ["mods"] = modsListToml,
                    }
                },                
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

        private static string GetProfilePath(string profileName)
        {
            return $".\\Profiles\\{profileName}.toml";
        }

        private static ProfileModel ReadProfileFromFile(string filePath)
        {
            if(!Directory.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            var profileName = Path.GetFileNameWithoutExtension(filePath);

            using StreamReader reader = File.OpenText(filePath);

            TomlTable table = TOML.Parse(reader);

            var enableModEngineDebug = table["modengine"]["debug"].AsBoolean;
            var externalDlls = new List<ExternalDllModel>();
            foreach (TomlNode node in table["modengine"]["external_dlls"])
            {
                var dll = node.AsString.Value;
                externalDlls.Add(new ExternalDllModel(node.AsString));
            }

            var enableModLoaderConfiguration = !table["extension"]["mod_loader"]["enabled"].AsBoolean;
            var modFolders = new List<ModModel>();
            foreach (TomlNode node in table["extension"]["mod_loader"]["mods"])
            {
                var enabled = node["enabled"].AsBoolean;
                var name = node["name"].AsString;
                var path = node["path"].AsString;

                modFolders.Add(new ModModel(name, path, enabled));
            }

            var enableScyllaHide = table["extension"]["scylla_hide"]["enabled"].AsBoolean;

            return new ProfileModel(
                profileName,
                modFolders,
                externalDlls,
                enableModEngineDebug,
                enableModLoaderConfiguration,
                enableScyllaHide);
        }
    }
}
