using ModEngine2ConfigTool.Models;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.ViewModels.Profiles
{
    public interface IProfileVm
    {
        DateTime Created { get; }
        string Description { get; set; }
        ObservableCollection<DllVm> ExternalDlls { get; }
        string ImagePath { get; set; }
        DateTime? LastPlayed { get; }
        Profile Model { get; }
        ObservableCollection<ModVm> Mods { get; }
        string Name { get; set; }
        bool UseDebugMode { get; set; }
        bool UseSaveManager { get; set; }
        bool UseScyllaHide { get; set; }

        Task RefreshAsync();
        void UpdateLastPlayed();
    }
}