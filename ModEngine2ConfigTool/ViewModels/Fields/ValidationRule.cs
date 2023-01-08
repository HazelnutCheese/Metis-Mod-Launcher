using System;

namespace ModEngine2ConfigTool.ViewModels.Fields
{
    public class ValidationRule<T>
    {
        private readonly Func<T, bool> _rule;

        private readonly string _errorMessage;

        public ValidationRule(
            Func<T, bool> rule, 
            string errorMessage)
        {
            _rule = rule;
            _errorMessage = errorMessage;
        }

        public string? Validate(T value)
        {
            return _rule(value)
                ? null
                : string.Format(_errorMessage, value);
        }
    }
}
