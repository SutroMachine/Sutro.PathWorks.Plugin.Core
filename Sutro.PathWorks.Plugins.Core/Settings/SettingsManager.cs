using gs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        protected virtual JsonSerializerSettings SerializerSettings()
        {
            return new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Error,
            };
        }

        public virtual void ApplyJSON(TSettings settings, string json)
        {
            JsonConvert.PopulateObject(json, settings, SerializerSettings());
        }

        public void ApplyJSON(IProfile settings, string json)
        {
            ApplyJSON((TSettings)settings, json);
        }

        public virtual void ApplyKeyValuePair(TSettings settings, string keyValue)
        {
            // TODO: Make this more strict to avoid converting values unintentionally
            var sFormatted = StringUtil.FormatSettingOverride(keyValue);
            JsonConvert.PopulateObject(sFormatted, settings, SerializerSettings());
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

        public TSettings DeserializeJSON(string json)
        {
            JObject o = JsonConvert.DeserializeObject<JObject>(json, SerializerSettings());
            var typeprop = o.Property("ClassTypeName");
            var typestring = typeprop.Value.Value<string>();
            var settings = CreateSettingsFromTypeName(typestring);
            JsonConvert.PopulateObject(json, settings);
            return settings;
        }

        private TSettings CreateSettingsFromTypeName(string typestring)
        {
            foreach (var setting in FactorySettings)
            {
                if (setting.GetType().Name == typestring.Split(".")[^1])
                {
                    return (TSettings)setting.Clone();
                }
            }
            throw new InvalidCastException($"Couldn't find factory setting of type {typestring}.");
        }

        public string SerializeJSON(TSettings settings)
        {
            return JsonConvert.SerializeObject(settings, SerializerSettings());
        }

        IProfile ISettingsManager.DeserializeJSON(string json)
        {
            return DeserializeJSON(json);
        }

        public string SerializeJSON(IProfile settings)
        {
            return SerializeJSON((TSettings)settings);
        }
    }
}