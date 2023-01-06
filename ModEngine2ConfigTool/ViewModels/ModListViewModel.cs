using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public abstract class ModListViewModel : ObservableObject
    {
        private ModViewModel? _selectedItem;

        public ICommand AddNewCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand MoveUpCommand { get; }

        public ICommand MoveDownCommand { get; }

        public ObservableCollection<ModViewModel> ProfileModsList { get; }

        public string Header { get; }

        public ModViewModel? SelectedItem 
        { 
            get => _selectedItem; 
            set => SetProperty(ref _selectedItem, value); 
        }

        public ModListViewModel(string header, IEnumerable<ModViewModel> modList)
        {
            AddNewCommand = new RelayCommand(AddNew);
            DeleteCommand = new RelayCommand(Delete);
            MoveUpCommand = new RelayCommand(MoveUp);
            MoveDownCommand = new RelayCommand(MoveDown);

            ProfileModsList = new ObservableCollection<ModViewModel>(modList);
            Header = header;
        }

        protected abstract void AddNew();

        private void Delete()
        {
            if (SelectedItem is null)
            {
                return;
            }

            ProfileModsList.Remove(SelectedItem);
        }

        private void MoveUp()
        {
            if(SelectedItem is null)
            {
                return;
            }

            var selectedIndex = ProfileModsList.IndexOf(SelectedItem);

            if(selectedIndex > 0)
            {
                ProfileModsList.Move(selectedIndex, selectedIndex - 1);
            }
        }

        private void MoveDown()
        {
            if (SelectedItem is null)
            {
                return;
            }

            var selectedIndex = ProfileModsList.IndexOf(SelectedItem);

            if (selectedIndex < ProfileModsList.Count - 1)
            {
                ProfileModsList.Move(selectedIndex, selectedIndex + 1);
            }
        }
    }
}
