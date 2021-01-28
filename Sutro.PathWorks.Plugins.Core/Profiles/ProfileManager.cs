using gs;
using Newtonsoft.Json;
using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.Core.UserSettings;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public abstract class ProfileManager<TProfile> : IProfileManager<TProfile> where TProfile : class, IProfile
    {
        public abstract List<TProfile> FactoryProfiles { get; }

        public abstract UserSettingCollectionBase<TProfile> UserSettings { get; }

        IUserSettingCollection IProfileManager<TProfile>.UserSettings => UserSettings;

        public virtual Result ApplyJSON(TProfile settings, string json)
        {
            JsonConvert.PopulateObject(json, settings, SerializerSettings());
            return Result.Ok();
        }

        public void ApplyKeyValuePair(TProfile settings, string keyValue)
        {
            // TODO: Make this more strict to avoid converting values unintentionally
            var sFormatted = StringUtil.FormatSettingOverride(keyValue);
            JsonConvert.PopulateObject(sFormatted, settings, SerializerSettings());
        }

        public virtual Result<TProfile> DeserializeJSON(string json)
        {
            var profile = JsonConvert.DeserializeObject<TProfile>(json, SerializerSettings());
            return Result<TProfile>.Ok(profile);
        }

        public string SerializeJSON(TProfile settings)
        {
            return JsonConvert.SerializeObject(settings, settings.GetType(), SerializerSettings());
        }

        protected virtual JsonSerializerSettings SerializerSettings()
        {
            var contractResolver = new Sutro.Core.Persistence.IgnoreablePropertiesContractResolver();
            contractResolver.Ignore(typeof(string), new string[] { "$schema" });

            return new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = contractResolver,
            };
        }
    }
}