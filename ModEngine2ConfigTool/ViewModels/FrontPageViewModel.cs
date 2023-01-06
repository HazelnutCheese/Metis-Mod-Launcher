using CommunityToolkit.Mvvm.ComponentModel;
using ModEngine2ConfigTool.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.ViewModels
{
    public class FrontPageViewModel : ObservableObject
    {
        public string PageName { get; } = nameof(FrontPageViewModel);

        public ProfileViewModel SelectedProfileViewModel { get; set; }
        public FrontPageViewModel()
        {
            SelectedProfileViewModel = new ProfileViewModel();

            var profile = ProfileService.ReadProfile("C:\\Users\\Thebb\\Desktop\\modengine2 test\\config_eldenring.toml");
        }
    }
}
