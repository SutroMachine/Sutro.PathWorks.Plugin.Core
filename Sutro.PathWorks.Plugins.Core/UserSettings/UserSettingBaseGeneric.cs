using Sutro.PathWorks.Plugins.API.Settings;
using System;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public abstract class UserSettingBase<TProfile, TValue> : UserSettingBase, IUserSettingGeneric<TValue> where TProfile : class
    {
        private readonly Func<TProfile, TValue> loadF;
        private readonly Action<TProfile, TValue> applyF;

        public TValue Value { get; set; }

        protected UserSettingBase(string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TProfile, TValue> loadF,
            Action<TProfile, TValue> applyF,
            Func<string> unitsF = null) : base(id, nameF, descriptionF, group, unitsF)
        {
            this.applyF = applyF;
            this.loadF = loadF;
        }

        public override void ApplyToRaw<T>(T profile)
        {
            ApplyToRaw(profile as TProfile);
        }

        public void ApplyToRaw(TProfile profile)
        {
            applyF(profile, Value);
        }

        public override void LoadFromRaw<T>(T profile)
        {
            LoadFromRaw(profile as TProfile);
        }

        public void LoadFromRaw(TProfile profile)
        {
            Value = loadF(profile);
        }

        public override void LoadAndApply<T>(T targetProfile, T sourceProfile)
        {
            var value = loadF(sourceProfile as TProfile);
            applyF(targetProfile as TProfile, value);
        }

        public TValue GetFromRaw(object settings)
        {
            return GetFromRaw((TProfile)settings);
        }

        public TValue GetFromRaw(TProfile profile)
        {
            return loadF(profile);
        }

        public void SetToRaw(object settings, TValue value)
        {
            SetToRaw((TProfile)settings, value);
        }

        public void SetToRaw(TProfile profile, TValue value)
        {
            applyF(profile, value);
        }
    }
}