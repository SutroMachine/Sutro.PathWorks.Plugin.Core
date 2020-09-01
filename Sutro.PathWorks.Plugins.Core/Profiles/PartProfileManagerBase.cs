using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public abstract class PartProfileManagerBase<TProfile> :
        ProfileManager<TProfile>, IProfileManager<IPartProfile>
        where TProfile : IPartProfile
    {
        List<IPartProfile> IProfileManager<IPartProfile>.FactoryProfiles =>
            FactoryProfiles.Select(p => (IPartProfile)p).ToList();

        IUserSettingCollection<IPartProfile> IProfileManager<IPartProfile>.UserSettings => throw new System.NotImplementedException();

        public void ApplyJSON(IPartProfile settings, string json)
        {
            ApplyJSON((TProfile)settings, json);
        }

        public void ApplyKeyValuePair(IPartProfile settings, string keyValue)
        {
            ApplyKeyValuePair((TProfile)settings, keyValue);
        }

        public string SerializeJSON(IPartProfile settings)
        {
            return ((ProfileManager<TProfile>)this).SerializeJSON((TProfile)settings);
        }

        IPartProfile IProfileManager<IPartProfile>.DeserializeJSON(string json)
        {
            return DeserializeJSON(json);
        }
    }
}