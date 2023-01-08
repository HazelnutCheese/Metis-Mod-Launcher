using System;
using System.IO;

namespace ModEngine2ConfigTool.ViewModels.Fields
{
    public static class CommonValidationRules
    {
        public static ValidationRule<string> NotEmpty(string errorMessage)
        {
            return new ValidationRule<string>(
                s => !string.IsNullOrWhiteSpace(s), 
                errorMessage);
        }

        public static ValidationRule<string> IsValidFilename(
            string errorMessageFormat,
            string? format = null)
        {
            return new ValidationRule<string>(
                s => IsValidFileName(s, format), 
                errorMessageFormat);
        }

        public static ValidationRule<string> DirectoryExists()
        {
            return new ValidationRule<string>(
                s => Directory.Exists(s), 
                "This directory does not exist.");
        }

        private static bool IsValidFileName(
            string value, 
            string? format = null) 
        {
            var fileName = format is null
                ? value
                : string.Format(format, value);

            try
            {
                File.OpenRead(fileName).Close();
            }
            catch (FileNotFoundException)
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
