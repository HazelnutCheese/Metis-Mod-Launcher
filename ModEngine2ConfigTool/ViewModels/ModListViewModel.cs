using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public abstract class ModListViewModel : ObservableObject
    {
        private ModViewModel? _selectedItem;

        public ICommand AddNewCommand { get; }

        public ICommand EditCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand MoveUpCommand { get; }

        public ICommand MoveDownCommand { get; }

        public ObservableCollection<ModViewModel> ProfileModsList { get; }

        public bool CanEdit { get; protected set; }

        public ModViewModel? SelectedItem 
        { 
            get => _selectedItem; 
            set => SetProperty(ref _selectedItem, value); 
        }

        public ModListViewModel(IEnumerable<ModViewModel> modList)
        {
            AddNewCommand = new RelayCommand(AddNew);
            EditCommand = new RelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            MoveUpCommand = new RelayCommand(MoveUp);
            MoveDownCommand = new RelayCommand(MoveDown);

            ProfileModsList = new ObservableCollection<ModViewModel>(modList);
        }

        protected abstract void AddNew();

        protected async virtual void Edit()
        {
            throw new NotImplementedException();
        }

        private void Delete()
        {
            if (SelectedItem is null)
            {
                return;
            }

            var selectedIndex = ProfileModsList.IndexOf(SelectedItem);
            ProfileModsList.Remove(SelectedItem);

            if(!ProfileModsList.Any())
            {
                return;
            }

            SelectedItem = selectedIndex < ProfileModsList.Count
                ? ProfileModsList[selectedIndex]
                : ProfileModsList[ProfileModsList.Count - 1];
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
