using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ModEngine2ConfigTool.ViewModels.Fields
{

    public class BaseFieldViewModel<T> : ObservableObject, IFieldViewModel
    {
        private T _value;

        private T? _defaultValue;

        public string Label { get; }

        public string ToolTip { get; }

        public T Value 
        { 
            get => _value;
            set
            {
                SetProperty(ref _value, value);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            }
        }

        public bool IsChanged => !Equals(_value, _defaultValue);

        public BaseFieldViewModel(
            string label, 
            string toolTip, 
            T defaultValue)
        {
            Label = label;
            ToolTip = toolTip;
            _defaultValue = defaultValue;
            _value = defaultValue;
        }

        public void AcceptChanges()
        {
            _defaultValue = _value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }
    }
}
