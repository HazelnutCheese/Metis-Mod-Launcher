using System.Collections.Generic;

namespace ModEngine2ConfigTool.Models
{
    public class ProfileModel
    {
        public string Name { get; }

        public List<ModModel> Mods { get; }

        public List<ModModel> Dlls { get; }

        public bool EnableME2Debug { get; }

        public bool EnableModLoaderConfiguration { get; }

        public bool EnableScyllaHide { get; }

        public ProfileModel(
            string name,
            List<ModModel> mods,
            List<ModModel> dlls,
            bool enableME2Debug,
            bool enableModLoaderConfiguration,
            bool enableScyllaHide)
        {
            Name = name;
            Mods = mods;
            Dlls = dlls;
            EnableME2Debug = enableME2Debug;
            EnableModLoaderConfiguration = enableModLoaderConfiguration;
            EnableScyllaHide = enableScyllaHide;
        }
    }
}
