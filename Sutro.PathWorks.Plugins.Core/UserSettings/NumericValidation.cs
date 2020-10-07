using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public static class NumericValidation
    {
        public static ValidationResult Validate<T>(NumericInfoBase<T> numericInfo, T value) where T : IComparable
        {
            if (numericInfo.Minimum != null)
            {
                int comparison = value.CompareTo(numericInfo.Minimum.Value);

                if (numericInfo.Minimum.Inclusive)
                {
                    if (comparison < 0)
                        return new ValidationResult(ValidationResultLevel.Error, $"Must be greater than or equal to {numericInfo.Minimum}");
                }
                else
                {
                    if (comparison <= 0)
                        return new ValidationResult(ValidationResultLevel.Error, $"Must be greater than {numericInfo.Minimum}");
                }
            }

            if (numericInfo.Maximum != null)
            {
                int comparison = value.CompareTo(numericInfo.Maximum.Value);

                if (numericInfo.Maximum.Inclusive)
                {
                    if (comparison > 0)
                        return new ValidationResult(ValidationResultLevel.Error, $"Must be less than or equal to {numericInfo.Maximum}");
                }
                else
                {
                    if (comparison >= 0)
                        return new ValidationResult(ValidationResultLevel.Error, $"Must be less than {numericInfo.Maximum}");
                }
            }

            // Successful result
            return new ValidationResult();
        }
    }
}