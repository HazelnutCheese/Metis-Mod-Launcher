﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Resx;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Pages
{
    public class HelpPageVm : ObservableObject
    {
        private readonly SaveManagerService _saveManagerService;

        public string AppName { get; }

        public string Version { get; }

        public string Author { get; }

        public string Licence { get; }

        public ICommand OpenBackupSavesFolderCommand { get; }


        public ObservableCollection<LicenceVm> ThirdPartyLicences { get; }

        public ObservableCollection<QuestionVm> FrequentlyAskedQuestions { get; }

        public HelpPageVm(Version version, SaveManagerService saveManagerService)
        {
            AppName = "Metis Mod Launcher";

            try
            {
                Version = version.ToString();
            }
            catch
            {
                Version= "Unknown";
            }

            _saveManagerService = saveManagerService;

            Author = "HazelnutCheese";

            Licence = Help.Licence_Metis;

            FrequentlyAskedQuestions = new ObservableCollection<QuestionVm>()
            {
                new QuestionVm(
                    "What is Metis Mod Launcher?",
                    Help.Faq_WhatIsMetisModLauncher),
                new QuestionVm(
                    "How do I download new mods?",
                    Help.Faq_HowToDownloadMods),
                new QuestionVm(
                    "How do I add a mod I have downloaded?",
                    Help.Faq_HowToImportMod),
                new QuestionVm(
                    "What is an external dll?",
                    Help.Faq_WhatIsAnExternalDll),
                new QuestionVm(
                    "How do I add an external dll?",
                    Help.Faq_HowToImportAnExternalDll),
                new QuestionVm(
                    "How can I tell if my mods have conflicts?",
                    Help.Faq_HowCanITellIfModsConflict),
                new QuestionVm(
                    "How can I merge mods to avoid conflicts?",
                    Help.Faq_HowToMergeMods),
                new QuestionVm(
                    "How do I make my profile use it's own save file?",
                    Help.Faq_ProfileSaves),
                new QuestionVm(
                    "How do I import saves from vanilla or another profile?",
                    Help.Faq_ImportSaves),
                new QuestionVm(
                    "I downloaded and imported a save but it's corrupt?",
                    Help.Faq_CorruptDownloadedSave),
                new QuestionVm(
                    "Where are saves automatically backed up to?",
                    Help.Faq_WhereAreSavedBackedUp),
                new QuestionVm(
                    "The mod or dll doesn't load or the game crashes",
                    Help.Faq_TheGameCrashesOrDoesntWork),
                //new QuestionVm(
                //    "How do I set a password for Seamless Coop?",
                //    Help.FaqCoopPassword),
                new QuestionVm(
                    "What does the Export Package option do?",
                    Help.Faq_WhatDoesExportPackageDo),
                new QuestionVm(
                    "Where does Metis store application data?",
                    Help.Faq_WhereDoesMetisStoreData),
                new QuestionVm(
                    "Where do I post bugs or issues I find?",
                    Help.Faq_IFoundABug),
            };

            ThirdPartyLicences = new ObservableCollection<LicenceVm>()
            {
                new LicenceVm(
                    "ModEngine2",
                    @"https://github.com/soulsmods/ModEngine2",
                    "Gary Tierney, katalash, Dasaav-dsv, horkrux, ivyl, ividyon",
                    "ModEngine-2.1.0.0-win64", 
                    Help.Licence_ModEngine2),
                new LicenceVm(
                    ".NET Community Toolkit",
                    @"https://github.com/CommunityToolkit/dotnet",
                    "Microsoft", 
                    "8.0.0", 
                    Help.Licence_WindowsCommunityToolkit),
                new LicenceVm(
                    "Material Design In XAML Toolkit",
                    @"https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit",
                    "James Willock", 
                    "4.7", 
                    Help.Licence_MaterialDesignInXamlToolkit),
                new LicenceVm(
                    "Tommy",
                    @"https://github.com/dezhidki/Tommy",
                    "Denis Zhidkikh", 
                    "3.1.2", 
                    Help.Licence_Tommy),
                new LicenceVm(
                    "FolderBrowserEx",
                    @"https://github.com/evaristocuesta/FolderBrowserEx",
                    "Evaristo Cuesta", 
                    "1.0.1", 
                    Help.Licence_FolderBrowserEx),
                new LicenceVm(
                    "Sherlog",
                    @"https://github.com/sschmid/Sherlog",
                    "Simon Schmid",
                    "1.0.0",
                    Help.Licence_Sherlog),
                new LicenceVm(
                    "CalcBinding",
                    @"https://github.com/Alex141/CalcBinding",
                    "Alexander Zinchenko",
                    "2.5.2",
                    Help.Licence_CalcBinding),
                new LicenceVm(
                    "Config.Net",
                    @"https://github.com/aloneguid/config",
                    "Ivan Gavryliuk (@aloneguid)",
                    "5.1.5",
                    Help.Licence_ConfigDotNet),
                new LicenceVm(
                    "Wix",
                    @"https://github.com/wixtoolset/wix3",
                    "Microsoft",
                    "3.11",
                    Help.Licence_Wix),
                new LicenceVm(
                    "Entity Framework Core",
                    @"https://learn.microsoft.com/en-gb/ef/core/",
                    "Microsoft",
                    "7.0.2",
                    Help.Licence_DotNetEf),
                new LicenceVm(
                    "Power Args",
                    @"https://github.com/adamabdelhamed/powerargs",
                    "Adam Abdelhamed",
                    "4.0.0",
                    Help.Licence_PowerArgs),
                new LicenceVm(
                    "Autofac",
                    @"https://github.com/autofac/Autofac",
                    "Autofac Contributors",
                    "8.0.0",
                    Help.Licence_Autofac)
            };

            OpenBackupSavesFolderCommand = new RelayCommand(OpenBackupSavesFolder);
            
        }

        private void OpenBackupSavesFolder()
        {
            _saveManagerService.OpenBackupSavesFolder();
        }
    }
}
