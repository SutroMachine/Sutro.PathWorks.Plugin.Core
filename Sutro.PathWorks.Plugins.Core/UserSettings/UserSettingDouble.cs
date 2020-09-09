using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingDouble<TSettings> : UserSetting<TSettings, double>
    {
        private readonly string range;

        public override string Range => range;

        public UserSettingDouble(
                    string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TSettings, double> loadF,
            Action<TSettings, double> applyF,
            double? minimumValue = null,
            double? maximumValue = null,
            ValidationResultLevel validationResultLevel = ValidationResultLevel.Error,
            Func<string> unitsF = null) : base(id, nameF, descriptionF, group, loadF, applyF, CreateValidationFunction(minimumValue, maximumValue, validationResultLevel), unitsF)
        {
            range = CreateRangeString(minimumValue, maximumValue);
        }

        private static string CreateRangeString(double? minimumValue, double? maximumValue)
        {
            if (minimumValue != null && maximumValue != null)
                return $"{minimumValue} - {maximumValue}";

            if (minimumValue == null)
                return $"<{maximumValue}";

            if (maximumValue == null)
                return $">{minimumValue}";

            return null;
        }

        private static Func<double, ValidationResult> CreateValidationFunction(double? minimumValue, double? maximumValue, ValidationResultLevel validationResultLevel)
        {
            if (minimumValue == null && maximumValue == null)
                return null;

            if (minimumValue == null)
                return UserSettingNumericValidations<double>.ValidateMax(maximumValue.Value, validationResultLevel);

            if (maximumValue == null)
                return UserSettingNumericValidations<double>.ValidateMin(minimumValue.Value, validationResultLevel);

            return UserSettingNumericValidations<double>.ValidateMinMax(minimumValue.Value, maximumValue.Value, validationResultLevel);
        }
    }
}