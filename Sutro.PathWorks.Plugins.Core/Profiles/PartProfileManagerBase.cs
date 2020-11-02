using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public abstract class PartProfileManagerBase<TProfile> :
        ProfileManager<TProfile>, IPartProfileManager
        where TProfile : class, IPartProfile
    {
        List<IPartProfile> IProfileManager<IPartProfile>.FactoryProfiles =>
            FactoryProfiles.Select(p => (IPartProfile)p).ToList();

        IUserSettingCollection IProfileManager<IPartProfile>.UserSettings => UserSettings;

        public void ApplyJSON(IPartProfile settings, string json)
        {
            base.ApplyJSON((TProfile)settings, json);
        }

        public void ApplyKeyValuePair(IPartProfile settings, string keyValue)
        {
            base.ApplyKeyValuePair((TProfile)settings, keyValue);
        }

        public string SerializeJSON(IPartProfile settings)
        {
            return base.SerializeJSON((TProfile)settings);
        }

        IPartProfile IProfileManager<IPartProfile>.DeserializeJSON(string json)
        {
            return base.DeserializeJSON(json);
        }
    }
}