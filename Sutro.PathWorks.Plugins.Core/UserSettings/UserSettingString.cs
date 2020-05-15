using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class UserSettingString<TSettings> : UserSetting<TSettings, string>
    {
        public UserSettingString(
            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TSettings, string> loadF,
            Action<TSettings, string> applyF,
            Func<string, ValidationResult> validateF = null) : base(id, nameF, descriptionF, group, loadF, applyF, validateF)
        {
        }
    }
}