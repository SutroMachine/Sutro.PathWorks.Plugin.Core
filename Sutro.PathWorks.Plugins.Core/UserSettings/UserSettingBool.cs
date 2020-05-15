using System;
using Sutro.PathWorks.Plugins.API.Settings;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingBool<TSettings> : UserSetting<TSettings, bool>
    {
        public UserSettingBool(
            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TSettings, bool> loadF,
            Action<TSettings, bool> applyF,
            Func<bool, ValidationResult> validateF = null) : base(id, nameF, descriptionF, group, loadF, applyF, validateF)
        {
        }
    }
}