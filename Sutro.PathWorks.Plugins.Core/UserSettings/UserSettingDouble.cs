using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingDouble<TProfile> : UserSettingBase<TProfile, double>, IUserSettingDouble where TProfile : class
    {
        public bool ConvertToPercentage { get; set; }

        public int DecimalDigits { get; set; }

        public NumericInfoDouble NumericInfo { get; set; }

        public UserSettingDouble(
                            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TProfile, double> loadF,
            Action<TProfile, double> applyF,
            Func<string> unitsF = null,
            NumericInfoDouble numericInfo = null,
            bool convertToPercentage = false,
            int decimalDigits = 2) :
            base(id, nameF, descriptionF, group, loadF, applyF, unitsF)
        {
            NumericInfo = numericInfo;
            ConvertToPercentage = convertToPercentage;
            DecimalDigits = decimalDigits;
        }

        public ValidationResult Validate()
        {
            return NumericValidation.Validate(NumericInfo, Value);
        }
    }
}