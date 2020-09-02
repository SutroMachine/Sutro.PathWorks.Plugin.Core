using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public abstract class MaterialProfileManagerBase<TProfile> :
        ProfileManager<TProfile>, IMaterialProfileManager
        where TProfile : IMaterialProfile
    {
        List<IMaterialProfile> IProfileManager<IMaterialProfile>.FactoryProfiles => FactoryProfiles.Select(p => (IMaterialProfile)p).ToList();

        IUserSettingCollection IProfileManager<IMaterialProfile>.UserSettings => UserSettings;

        public void ApplyJSON(IMaterialProfile settings, string json)
        {
            ApplyJSON((TProfile)settings, json);
        }

        public void ApplyKeyValuePair(IMaterialProfile settings, string keyValue)
        {
            ApplyKeyValuePair((TProfile)settings, keyValue);
        }

        public string SerializeJSON(IMaterialProfile settings)
        {
            return ((ProfileManager<TProfile>)this).SerializeJSON((TProfile)settings);
        }

        IMaterialProfile IProfileManager<IMaterialProfile>.DeserializeJSON(string json)
        {
            return DeserializeJSON(json);
        }
    }
}