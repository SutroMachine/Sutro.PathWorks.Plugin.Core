using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public abstract class UserSettingBase : IUserSetting
    {
        public Func<string> NameF { get; set; }
        public Func<string> DescriptionF { get; set; }
        public Func<string> UnitsF { get; set; }

        public string Name => NameF?.Invoke();
        public string Description => DescriptionF?.Invoke();
        public string Units => UnitsF?.Invoke();

        public string Id { get; set; }
        public bool Hidden { get; set; } = false;

        public IUserSettingGroup Group { get; set; }

        public abstract void ApplyToRaw<T>(T profile) where T : IProfile;

        public abstract void LoadFromRaw<T>(T profile) where T : IProfile;

        public abstract void LoadAndApply<T>(T targetProfile, T sourceProfile) where T : IProfile;

        protected UserSettingBase(string id,
            Func<string> nameF,
            Func<string> descriptionF = null,
            UserSettingGroup group = null,
            Func<string> unitsF = null)
        {
            Id = id;
            NameF = nameF;
            DescriptionF = descriptionF;
            Group = group;
            UnitsF = unitsF;
        }
    }
}