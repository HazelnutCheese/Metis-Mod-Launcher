using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModEngine2ConfigTool.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        public string PageName { get; } = nameof(SettingsViewModel);
    }
}
