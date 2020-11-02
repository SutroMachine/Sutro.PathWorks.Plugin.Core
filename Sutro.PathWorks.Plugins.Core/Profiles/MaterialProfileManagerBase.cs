using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public abstract class MaterialProfileManagerBase<TProfile> :
        ProfileManager<TProfile>, IMaterialProfileManager
        where TProfile : class, IMaterialProfile
    {
        List<IMaterialProfile> IProfileManager<IMaterialProfile>.FactoryProfiles => FactoryProfiles.Select(p => (IMaterialProfile)p).ToList();

        IUserSettingCollection IProfileManager<IMaterialProfile>.UserSettings => UserSettings;

        public void ApplyJSON(IMaterialProfile settings, string json)
        {
            base.ApplyJSON((TProfile)settings, json);
        }

        public void ApplyKeyValuePair(IMaterialProfile settings, string keyValue)
        {
            base.ApplyKeyValuePair((TProfile)settings, keyValue);
        }

        public string SerializeJSON(IMaterialProfile settings)
        {
            return base.SerializeJSON((TProfile)settings);
        }

        IMaterialProfile IProfileManager<IMaterialProfile>.DeserializeJSON(string json)
        {
            return base.DeserializeJSON(json);
        }
    }
}