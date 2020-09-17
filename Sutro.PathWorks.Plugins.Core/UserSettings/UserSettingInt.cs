using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingInt<TSettings> : UserSetting<TSettings, int>
    {
        private readonly string rangeText;
        private readonly string rangeMin;
        private readonly string rangeMax;

        public override string RangeText => rangeText;
        public override string RangeMin => rangeMin;
        public override string RangeMax => rangeMax;

        public UserSettingInt(
                    string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TSettings, int> loadF,
            Action<TSettings, int> applyF,
            int? minimumValue = null,
            int? maximumValue = null,
            ValidationResultLevel validationResultLevel = ValidationResultLevel.Error,
            Func<string> unitsF = null) : base(id, nameF, descriptionF, group, loadF, applyF, CreateValidationFunction(minimumValue, maximumValue, validationResultLevel),
                unitsF)
        {
            rangeText = CreateRangeString(minimumValue, maximumValue);
            rangeMin = minimumValue?.ToString();
            rangeMax = maximumValue?.ToString();
        }

        private static string CreateRangeString(int? minimumValue, int? maximumValue)
        {
            if (minimumValue != null && maximumValue != null)
                return $"{minimumValue} - {maximumValue}";

            if (minimumValue == null)
                return $"<{maximumValue}";

            if (maximumValue == null)
                return $">{minimumValue}";

            return null;
        }

        private static Func<int, ValidationResult> CreateValidationFunction(int? minimumValue, int? maximumValue, ValidationResultLevel validationResultLevel)
        {
            if (minimumValue == null && maximumValue == null)
                return null;

            if (minimumValue == null)
                return UserSettingNumericValidations<int>.ValidateMax(maximumValue.Value, validationResultLevel);

            if (maximumValue == null)
                return UserSettingNumericValidations<int>.ValidateMin(minimumValue.Value, validationResultLevel);

            return UserSettingNumericValidations<int>.ValidateMinMax(minimumValue.Value, maximumValue.Value, validationResultLevel);
        }
    }
}