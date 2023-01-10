using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Xaml.Behaviors.Media;
using ModEngine2ConfigTool.ViewModels.ProfileComponents;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace ModEngine2ConfigTool.ViewModels.Profiles
{
    public class ProfileVm : ObservableObject
    {
        private string _description;
        private string _name;
        private string _imagePath;

        public string Name
        {
            get => _name;
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _name, value);
                }
            }
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public DateTime? LastPlayed { get; }

        public DateTime Created { get; }

        public ObservableCollection<ModVm> Mods { get; }

        public ObservableCollection<DllVm> ExternalDlls { get; }

        public ICommand AddModCommand { get; }

        public ICommand RemoveModCommand { get; }

        public ProfileVm(string name)
        {
            _name = name;
            _description = string.Empty;
            _imagePath = string.Empty;

            Created = DateTime.Now;

            Mods = new ObservableCollection<ModVm>();
            ExternalDlls= new ObservableCollection<DllVm>();

            AddModCommand = new AsyncRelayCommand<ModVm>(AddMod);
            RemoveModCommand = new AsyncRelayCommand<ModVm>(RemoveMod);
        }

        public ProfileVm(
            string name,
            string description,
            string imagePath,
            DateTime created,
            DateTime? lastPlayed = null)
        {
            _name = name;
            _description = description;
            _imagePath = imagePath;

            Created = created;
            LastPlayed = lastPlayed;

            Mods = new ObservableCollection<ModVm>();
            ExternalDlls = new ObservableCollection<DllVm>();

            AddModCommand = new AsyncRelayCommand<ModVm>(AddMod);
            RemoveModCommand = new AsyncRelayCommand<ModVm>(RemoveMod);
        }

        private async Task AddMod(ModVm? mod)
        {
            if(mod is null)
            {
                return;
            }

            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if(!Mods.Contains(mod))
                {
                    Mods.Add(mod);
                }
            });
        }

        private async Task RemoveMod(ModVm? mod)
        {
            if (mod is null)
            {
                return;
            }

            await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                if (Mods.Contains(mod))
                {
                    Mods.Remove(mod);
                }
            });
        }
    }
}
