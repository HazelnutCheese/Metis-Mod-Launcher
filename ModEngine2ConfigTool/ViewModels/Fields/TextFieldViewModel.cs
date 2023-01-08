using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ModEngine2ConfigTool.ViewModels.Fields
{

    public class TextFieldViewModel : BaseFieldViewModel<string>, INotifyDataErrorInfo
    {
        private readonly List<ValidationRule<string>>? _validationRules;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasButton { get; }
        public string? ButtonLabel { get; }
        public ICommand? ButtonCommand { get; }

        public bool HasErrors
        {
            get
            {
                return GetErrors(nameof(Value)).Cast<string>().Any();
            }
        }

        public TextFieldViewModel(
            string label,
            string toolTip,
            string defaultValue,
            List<ValidationRule<string>>? validationRules = null) : base(
                label, 
                toolTip, 
                defaultValue)
        {
            HasButton = false;
            ButtonLabel = null;
            ButtonCommand = null;

            _validationRules = validationRules;
            PropertyChanged += OnPropertyChanged;
        }

        public TextFieldViewModel(
            string label,
            string toolTip,
            string defaultValue,
            string buttonName,
            ICommand buttonCommand,
            List<ValidationRule<string>>? validationRules = null) : base(
                label,
                toolTip,
                defaultValue)
        {
            HasButton = true;
            ButtonLabel = buttonName;
            ButtonCommand = buttonCommand;

            _validationRules = validationRules;
            PropertyChanged += OnPropertyChanged;
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if(Equals(propertyName, nameof(Value)) && _validationRules is not null)
            {
                return _validationRules
                    .Select(x => x.Validate(Value))
                    .Where(x => x is not null);
            }

            return new List<string?>();
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(e.PropertyName));
        }
    }
}
