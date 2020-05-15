using gs;
using Newtonsoft.Json;
using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public abstract class SettingsManager<TSettings> : ISettingsManager<TSettings>
        where TSettings : IPlanarAdditiveSettings, IProfile
    {
        public abstract List<TSettings> FactorySettings { get; }
        public abstract IUserSettingCollection<TSettings> MachineUserSettings { get; }
        public abstract IUserSettingCollection<TSettings> MaterialUserSettings { get; }
        public abstract IUserSettingCollection<TSettings> PrintUserSettings { get; }

        List<IProfile> ISettingsManager.FactorySettings
        {
            get
            {
                var settings = new List<IProfile>();
                foreach (TSettings setting in FactorySettings)
                    settings.Add(setting);
                return settings;
            }
        }

        IUserSettingCollection ISettingsManager.MachineUserSettings => MachineUserSettings;
        IUserSettingCollection ISettingsManager.MaterialUserSettings => MaterialUserSettings;
        IUserSettingCollection ISettingsManager.PrintUserSettings => PrintUserSettings;

        private readonly static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error,
        };

        public virtual void ApplyJSON(TSettings settings, string json)
        {
            JsonConvert.PopulateObject(json, settings, jsonSerializerSettings);
        }

        public void ApplyJSON(IProfile settings, string json)
        {
            ApplyJSON((TSettings)settings, json);
        }

        public virtual void ApplyKeyValuePair(TSettings settings, string keyValue)
        {
            // TODO: Make this more strict to avoid converting values unintentionally
            var sFormatted = StringUtil.FormatSettingOverride(keyValue);
            JsonConvert.PopulateObject(sFormatted, settings, jsonSerializerSettings);
        }

        public void ApplyKeyValuePair(IProfile settings, string keyValue)
        {
            ApplyKeyValuePair((TSettings)settings, keyValue);
        }

        public virtual TSettings FactorySettingByManufacturerAndModel(string manufacturer, string model)
        {
            Func<string, string> SimplifyName = (s) => s.Replace(" ", "").Replace("_", "").ToLower();

            Func<string, string, bool> MatchName = (a, b) => SimplifyName(a) == SimplifyName(b);

            var profiles = (from profile in FactorySettings
                            where MatchName(profile.BaseMachine.ManufacturerName, manufacturer) &&
                                  MatchName(profile.BaseMachine.ModelIdentifier, model)
                            select profile).ToArray();

            if (profiles.Length == 0)
                throw new KeyNotFoundException($"Matching profile not found for: {manufacturer} {model}");

            return profiles[0];
        }

        IProfile ISettingsManager.FactorySettingByManufacturerAndModel(string manufacturer, string model)
        {
            return FactorySettingByManufacturerAndModel(manufacturer, model);
        }
    }
}