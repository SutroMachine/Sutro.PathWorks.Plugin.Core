using System;
using Sutro.PathWorks.Plugins.API.Settings;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    public class UserSetting<TSettings, TValue> : UserSettingBase<TSettings>
    {
        public TValue Value { get; set; }

        private readonly Func<TValue, ValidationResult> validateF;
        private readonly Func<TSettings, TValue> loadF;
        private readonly Action<TSettings, TValue> applyF;

        public UserSetting(
            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TSettings, TValue> loadF,
            Action<TSettings, TValue> applyF,
            Func<TValue, ValidationResult> validateF = null) : base(id, nameF, descriptionF, group)
        {
            this.validateF = validateF;
            this.applyF = applyF;
            this.loadF = loadF;
        }

        public override void LoadFromRaw(TSettings settings)
        {
            Value = loadF(settings);
        }

        public override void ApplyToRaw(TSettings settings)
        {
            applyF(settings, Value);
        }

        public override object GetFromRaw(TSettings settings)
        {
            return loadF(settings);
        }

        public override void SetToRaw(TSettings settings, object value)
        {
            TValue tValue;
            try
            {
                tValue = (TValue)Convert.ChangeType(value, typeof(TValue));
            }
            catch (Exception e)
            {
                throw new InvalidCastException($"Setting {Name}: Function SetToRaw received an object of type {value.GetType()}, expected {typeof(TValue)}.", e);
            }
            applyF(settings, tValue);
        }

        public override ValidationResult Validation
        {
            get
            {
                if (validateF != null)
                    return validateF(Value);
                return new ValidationResult();
            }
        }

        public override ValidationResult Validate(object value)
        {
            if (value is TValue tValue)
            {
                if (validateF != null)
                    return validateF(tValue);
                return new ValidationResult();
            }
            return new ValidationResult(ValidationResultLevel.Error, "Invalid cast");
        }
    }
}