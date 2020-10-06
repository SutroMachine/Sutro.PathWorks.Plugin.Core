using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingBool<TProfile> : UserSettingBase<TProfile, bool>, IUserSettingBool where TProfile : class
    {
        public UserSettingBool(
            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TProfile, bool> loadF,
            Action<TProfile, bool> applyF) : base(id, nameF, descriptionF, group, loadF, applyF)
        {
        }
    }
}