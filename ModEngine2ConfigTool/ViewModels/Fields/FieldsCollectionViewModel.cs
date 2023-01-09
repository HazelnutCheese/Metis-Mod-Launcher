using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ModEngine2ConfigTool.ViewModels.Fields
{

    public class FieldsCollectionViewModel : ObservableObject, IChangeTracking
    {
        public ObservableCollection<IFieldViewModel> Fields { get; }

        public bool IsChanged => Fields.Any(x => x.IsChanged);

        public FieldsCollectionViewModel(List<IFieldViewModel> fields) 
        {
            Fields = new ObservableCollection<IFieldViewModel>(fields);

            foreach(var field in Fields)
            {
                field.PropertyChanged += Field_Changed;
            }
        }

        private void Field_Changed(object? sender, PropertyChangedEventArgs e)
        {
            if(Equals(e.PropertyName, nameof(IIsChanged.IsChanged)))
            {
                OnPropertyChanged(nameof(IsChanged));
            }
        }

        public void AcceptChanges()
        {
            foreach(var field in Fields)
            {
                field.AcceptChanges();
            }
        }

        public T GetField<T>(string fieldName) where T : IFieldViewModel
        {
            return Fields.OfType<T>().Single(x => x.Label == fieldName);
        }
    }
}
