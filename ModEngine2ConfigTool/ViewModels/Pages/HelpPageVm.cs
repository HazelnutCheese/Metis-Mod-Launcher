using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Resx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class HelpPageVm : ObservableObject
    {
        public string AppName { get; }

        public string Version { get; }

        public string Author { get; }

        public string Licence { get; }

        public string FaqWhatIsMetis { get; }

        public string FaqHowToDownloadMods { get; }

        public string FaqHowToImportMods { get; }

        public string FaqWhatIsAnExternalDll { get; }

        public string FaqHowToImportExternalDlls { get; }

        public string FaqWhatDoesImportingDo { get; }

        public string FaqWhatHappensIfIMoveAModOrDll { get; }

        public string FaqWhereDoesMetisStoreData { get; }

        public string FaqHowToMergeMods { get; }

        public string FaqEnableProfileSaves { get; }

        public string FaqImportProfileSaves { get; }

        public string FaqDoModsConflict { get; }

        public string FaqIFoundABug { get; }

        public string FaqCoopPassword { get; }

        public string LicenceModEngine2 { get; }

        public string LicenceWindowsCommunityToolkit { get; }

        public string LicenceMaterialDesignInXamlToolkit { get; }

        public string LicenceTommy { get; }

        public string LicenceFolderBrowserEx { get; }

        public string LicenceSherlog { get; }

        public string LicenceCalBinding { get; }

        public string LicenceConfigDotNet { get; }

        public string LicenceWix { get; }

        public string LicenceDotNotEntityFramework { get; }

        public HelpPageVm()
        {
            AppName = "Metis Mod Launcher";

            try
            {
                Version = typeof(App).Assembly
                    .GetName()
                    .Version?
                    .ToString() ?? "No Version";
            }
            catch
            {
                Version= "Unknown";
            }

            Author = "HazelnutCheese";

            Licence = Help.Licence_Metis;

            FaqWhatIsMetis = Help.Faq_WhatIsMetisModLauncher;
            FaqHowToDownloadMods = Help.Faq_HowToDownloadMods;
            FaqHowToImportMods = Help.Faq_HowToImportMod;
            FaqWhatIsAnExternalDll = Help.Faq_WhatIsAnExternalDll;
            FaqHowToImportExternalDlls = Help.Faq_HowToImportAnExternalDll;
            FaqWhatDoesImportingDo = Help.Faq_WhatDoesImportingDo;
            FaqWhatHappensIfIMoveAModOrDll = Help.Faq_WhatHappensIfIMoveAModOrDllAfterImport;
            FaqWhereDoesMetisStoreData = Help.Faq_WhereDoesMetisStoreData;
            FaqIFoundABug = Help.Faq_IFoundABug;
            FaqHowToMergeMods = Help.Faq_HowToMergeMods;
            FaqDoModsConflict = Help.Faq_HowCanITellIfModsConflict;
            FaqEnableProfileSaves = Help.Faq_ProfileSaves;
            FaqImportProfileSaves = Help.Faq_ImportSaves;
            FaqCoopPassword = Help.FaqCoopPassword;

            LicenceModEngine2 = Help.Licence_ModEngine2;
            LicenceWindowsCommunityToolkit = Help.Licence_WindowsCommunityToolkit;
            LicenceMaterialDesignInXamlToolkit = Help.Licence_MaterialDesignInXamlToolkit;
            LicenceTommy = Help.Licence_Tommy;
            LicenceFolderBrowserEx = Help.Licence_FolderBrowserEx;
            LicenceSherlog = Help.Licence_Sherlog;
            LicenceCalBinding = Help.Licence_CalcBinding;
            LicenceConfigDotNet = Help.Licence_ConfigDotNet;
            LicenceWix = Help.Licence_Wix;
            LicenceDotNotEntityFramework = Help.Licence_DotNetEf;
        }
    }
}
