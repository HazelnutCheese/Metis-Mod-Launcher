﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.Helpers;
using ModEngine2ConfigTool.Services;
using ModEngine2ConfigTool.ViewModels.Pages;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using ModEngine2ConfigTool.ViewModels.Profiles;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public class MainPanelVm : ObservableObject
    {
        private ProfileManagerService _profileManagerService;
        private ModManagerService _modManagerService;
        private ProfileVm? _selectedItem;

        public NavigationService Navigator { get; }

        public ProfileVm? SelectedItem 
        { 
            get => _selectedItem; 
            private set => SetProperty(ref _selectedItem, value); 
        }

        public MainPanelVm(NavigationService navigator)
        {
            _profileManagerService = new ProfileManagerService(
                App.DatabaseService,
                App.DispatcherService);

            _modManagerService = new ModManagerService(
                App.DatabaseService,
                App.DispatcherService,
                _profileManagerService);

            Navigator = navigator;

            Navigator.CurrentPage = new HomePageVm(
                Navigator, 
                _profileManagerService, 
                _modManagerService);
        }
    }
}
