using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public abstract class MachineProfileManagerBase<TProfile> :
        ProfileManager<TProfile>, IMachineProfileManager
        where TProfile : IMachineProfile
    {
        List<IMachineProfile> IProfileManager<IMachineProfile>.FactoryProfiles => 
            FactoryProfiles.Select(p => (IMachineProfile)p).ToList();

        IUserSettingCollection IProfileManager<IMachineProfile>.UserSettings => UserSettings;

        public void ApplyJSON(IMachineProfile settings, string json)
        {
            ApplyJSON((TProfile)settings, json);
        }

        public void ApplyKeyValuePair(IMachineProfile settings, string keyValue)
        {
            ApplyKeyValuePair((TProfile)settings, keyValue);
        }

        public string SerializeJSON(IMachineProfile settings)
        {
            return ((ProfileManager<TProfile>)this).SerializeJSON((TProfile)settings);
        }

        IMachineProfile IProfileManager<IMachineProfile>.DeserializeJSON(string json)
        {
            return DeserializeJSON(json);
        }
    }
}