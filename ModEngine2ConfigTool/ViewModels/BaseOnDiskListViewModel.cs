using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModEngine2ConfigTool.ViewModels.Fields;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels
{
    public abstract class BaseOnDiskListViewModel<T> : ObservableObject where T : IOnDiskViewModel
    {
        private T? _selectedItem;
        private List<T> _originalItems;

        public ICommand AddNewCommand { get; }

        public ICommand EditCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand MoveUpCommand { get; }

        public ICommand MoveDownCommand { get; }

        public ObservableCollection<T> OnDiskObjectList { get; }

        public bool CanEdit { get; protected set; }

        public virtual bool IsChanged => !OnDiskObjectList.SequenceEqual(_originalItems);

        public T? SelectedItem 
        { 
            get => _selectedItem; 
            set => SetProperty(ref _selectedItem, value); 
        }

        public BaseOnDiskListViewModel(List<T> items)
        {
            AddNewCommand = new RelayCommand(AddNew);
            EditCommand = new AsyncRelayCommand(Edit);
            DeleteCommand = new RelayCommand(Delete);
            MoveUpCommand = new RelayCommand(MoveUp);
            MoveDownCommand = new RelayCommand(MoveDown);

            _originalItems = items;
            OnDiskObjectList = new ObservableCollection<T>(items);
            OnDiskObjectList.CollectionChanged += ProfileModsList_CollectionChanged;

            foreach (var mod in OnDiskObjectList)
            {
                mod.PropertyChanged += Mod_Changed;
            }
        }

        private void ProfileModsList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsChanged));
        }

        protected void Mod_Changed(object? sender, PropertyChangedEventArgs e)
        {
            if (Equals(e.PropertyName, nameof(IIsChanged.IsChanged)))
            {
                OnPropertyChanged(nameof(IsChanged));
            }
        }

        protected abstract void AddNew();

        protected async virtual Task Edit()
        {
            await Task.FromException(new NotImplementedException());
        }

        private void Delete()
        {
            if (SelectedItem is null)
            {
                return;
            }

            var selectedIndex = OnDiskObjectList.IndexOf(SelectedItem);
            SelectedItem.PropertyChanged -= Mod_Changed;
            OnDiskObjectList.Remove(SelectedItem);

            if(!OnDiskObjectList.Any())
            {
                return;
            }

            SelectedItem = selectedIndex < OnDiskObjectList.Count
                ? OnDiskObjectList[selectedIndex]
                : OnDiskObjectList[OnDiskObjectList.Count - 1];
        }

        private void MoveUp()
        {
            if(SelectedItem is null)
            {
                return;
            }

            var selectedIndex = OnDiskObjectList.IndexOf(SelectedItem);

            if(selectedIndex > 0)
            {
                OnDiskObjectList.Move(selectedIndex, selectedIndex - 1);
            }
        }

        private void MoveDown()
        {
            if (SelectedItem is null)
            {
                return;
            }

            var selectedIndex = OnDiskObjectList.IndexOf(SelectedItem);

            if (selectedIndex < OnDiskObjectList.Count - 1)
            {
                OnDiskObjectList.Move(selectedIndex, selectedIndex + 1);
            }
        }
    }
}
