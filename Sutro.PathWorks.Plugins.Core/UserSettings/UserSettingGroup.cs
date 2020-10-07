using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingGroup : IUserSettingGroup
    {
        public Func<string> NameF { get; set; }

        public string Name => NameF();

        public UserSettingGroup(Func<string> nameF, Func<string> descriptionF = null)
        {
            NameF = nameF;
        }
    }
}