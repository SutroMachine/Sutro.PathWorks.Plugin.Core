using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingInt<TProfile> : UserSettingBase<TProfile, int>, IUserSettingInt where TProfile : class
    {
        public NumericInfoInt NumericInfo { get; }

        public UserSettingInt(
            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TProfile, int> loadF,
            Action<TProfile, int> applyF,
            Func<string> unitsF = null,
            NumericInfoInt numericInfo = null) :
            base(id, nameF, descriptionF, group, loadF, applyF, unitsF)
        {
            NumericInfo = numericInfo;
        }

        public ValidationResult Validate()
        {
            return NumericValidation.Validate(NumericInfo, Value);
        }
    }
}