using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public static class UserSettingStringValidationsFactory
    {
        public static Func<string, ValidationResult> IsNotEmpty()
        {
            return (s) => string.IsNullOrWhiteSpace(s) ? new ValidationResult(ValidationResultLevel.Error, "String cannot be empty") : new ValidationResult();
        }

        public static Func<string, ValidationResult> DoesNotContainWhitespace()
        {
            return (s) => (s.Split().Length > 1) ? new ValidationResult(ValidationResultLevel.Error, "String cannot contain whitespace") : new ValidationResult();
        }
    }
}