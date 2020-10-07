using Sutro.PathWorks.Plugins.API.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingEnum<TProfile> : UserSettingBase<TProfile, int>, IUserSettingEnum where TProfile : class
    {
        public UserSettingEnum(
             string id,
             Func<string> nameF,
             Func<string> descriptionF,
             UserSettingGroup group,
             Func<TProfile, int> loadF,
             Action<TProfile, int> applyF,
             IEnumerable<UserSettingEnumOption> options) :
             base(id, nameF, descriptionF, group, loadF, applyF)
        {
            Options = new ReadOnlyCollection<UserSettingEnumOption>(options.ToList());
        }

        public ReadOnlyCollection<UserSettingEnumOption> Options { get; }
    }
}