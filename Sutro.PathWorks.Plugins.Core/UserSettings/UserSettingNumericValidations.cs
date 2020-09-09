using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public static class UserSettingNumericValidations<T> where T : IComparable<T>
    {
        public static Func<T, ValidationResult> ValidateMax(T max, ValidationResultLevel level)
        {
            return (val) =>
            {
                if (val.CompareTo(max) > 0)
                    return new ValidationResult(level, string.Format("Must be no more than {0}", max));
                return new ValidationResult();
            };
        }

        public static Func<T, ValidationResult> ValidateMin(T min, ValidationResultLevel level)
        {
            return (val) =>
            {
                if (val.CompareTo(min) < 0)
                    return new ValidationResult(level, string.Format("Must be at least {0}", min));
                return new ValidationResult();
            };
        }

        public static Func<T, ValidationResult> ValidateMinMax(T min, T max, ValidationResultLevel level)
        {
            return (val) =>
            {
                var result = ValidateMin(min, level).Invoke(val);
                if (result.Severity == ValidationResultLevel.Message)
                    result = ValidateMax(max, level).Invoke(val);
                return result;
            };
        }
    }
}