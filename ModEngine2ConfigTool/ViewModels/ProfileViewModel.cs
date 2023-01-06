using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace ModEngine2ConfigTool.ViewModels
{

    public class ProfileViewModel : ObservableObject
    {
        public ModFolderListViewModel ModFolderListViewModel { get; set; }

        public DllListViewModel DllListViewModel { get; set; }

        public ProfileViewModel()
        {
            ModFolderListViewModel = new ModFolderListViewModel("Mods", Array.Empty<ModViewModel>());
            DllListViewModel = new DllListViewModel("External Dlls", Array.Empty<ModViewModel>());
        }
    }
}
