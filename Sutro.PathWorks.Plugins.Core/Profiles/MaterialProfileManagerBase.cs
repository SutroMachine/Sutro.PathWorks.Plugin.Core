using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API;
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

        public Result ApplyJSON(IMaterialProfile settings, string json)
        {
            return base.ApplyJSON((TProfile)settings, json);
        }

        public void ApplyKeyValuePair(IMaterialProfile settings, string keyValue)
        {
            base.ApplyKeyValuePair((TProfile)settings, keyValue);
        }

        public void OnSet(IMaterialProfile profile)
        {
            base.OnSet((TProfile)profile);
        }

        public string SerializeJSON(IMaterialProfile settings)
        {
            return base.SerializeJSON((TProfile)settings);
        }

        Result<IMaterialProfile> IProfileManager<IMaterialProfile>.DeserializeJSON(string json)
        {
            var result = base.DeserializeJSON(json);
            return result.IsSuccessful ?
                Result<IMaterialProfile>.Ok(result.Value, result.Warnings) :
                Result<IMaterialProfile>.Fail(result.Error);
        }
    }
}