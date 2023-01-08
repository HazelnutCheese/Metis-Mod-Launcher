using System.Collections.Generic;

namespace ModEngine2ConfigTool.Models
{
    public class ProfileModel
    {
        public string Name { get; }

        public List<ModModel> Mods { get; }

        public List<ExternalDllModel> Dlls { get; }

        public bool EnableME2Debug { get; }

        public bool IgnoreModFolders { get; }

        public bool EnableScyllaHide { get; }

        public ProfileModel(
            string name,
            List<ModModel> mods,
            List<ExternalDllModel> dlls,
            bool enableME2Debug,
            bool ignoreModFolders,
            bool enableScyllaHide)
        {
            Name = name;
            Mods = mods;
            Dlls = dlls;
            EnableME2Debug = enableME2Debug;
            IgnoreModFolders = ignoreModFolders;
            EnableScyllaHide = enableScyllaHide;
        }

        public ProfileModel(string name)
        {
            Name = name;
            Mods = new List<ModModel>();
            Dlls = new List<ExternalDllModel>();
            IgnoreModFolders = false;
        }
    }
}
