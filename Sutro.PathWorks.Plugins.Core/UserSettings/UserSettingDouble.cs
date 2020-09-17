using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingDouble<TSettings> : UserSetting<TSettings, double>
    {
        private readonly string rangeText;
        private readonly string rangeMin;
        private readonly string rangeMax;

        public override string RangeText => rangeText;
        public override string RangeMin => rangeMin;
        public override string RangeMax => rangeMax;

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
            rangeText = CreateRangeString(minimumValue, maximumValue);
            rangeMin = minimumValue?.ToString();
            rangeMax = maximumValue?.ToString();
        }

        private static string CreateRangeString(double? minimumValue, double? maximumValue)
        {
            if (minimumValue.HasValue && maximumValue.HasValue)
                return $"{minimumValue} - {maximumValue}";

            if (maximumValue.HasValue)
                return $"<{maximumValue}";

            if (minimumValue.HasValue)
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