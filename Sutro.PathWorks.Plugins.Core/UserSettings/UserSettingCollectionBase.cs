using Sutro.Core.Models.Profiles;
using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public abstract class UserSettingCollectionBase<TProfile> : IUserSettingCollection where TProfile : class, IProfile
    {
        /// <summary>
        /// Provides iteration through user settings typed with underlying raw settings type.
        /// </summary>
        public IEnumerable<IUserSetting> Settings()
        {
            foreach (var setting in EnumerateProperties())
                yield return setting;

            foreach (var setting in EnumerateFields())
                yield return setting;
        }

        private IEnumerable<IUserSetting> EnumerateProperties()
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                if (typeof(IUserSetting).IsAssignableFrom(property.PropertyType))
                {
                    var setting = (IUserSetting)property.GetValue(this);
                    if (!setting.Hidden)
                        yield return setting;
                }
            }
        }

        private IEnumerable<IUserSetting> EnumerateFields()
        {
            foreach (FieldInfo field in GetType().GetFields())
            {
                if (typeof(IUserSetting).IsAssignableFrom(field.FieldType))
                {
                    var setting = (IUserSetting)field.GetValue(this);
                    if (!setting.Hidden)
                        yield return setting;
                }
            }
        }

        /// <summary>
        /// Checks the values of all user settings.
        /// </summary>
        /// <remarks>
        /// This method can be overridden in derived classes to add validations
        /// between combinations of user settings in addition to the individual checks.
        /// </remarks>
        public virtual List<ValidationResult> Validate(TProfile settings)
        {
            return new List<ValidationResult>();
        }

        /// <summary>
        /// Checks the individual validations of each user setting.
        /// </summary>
        public virtual List<ValidationResult> ValidateIndivual(TProfile settings)
        {
            var validations = new List<ValidationResult>();
            foreach (var userSetting in Settings())
            {
                userSetting.LoadFromRaw(settings);

                var validation = userSetting switch
                {
                    IUserSettingString o => o.Validate(),
                    IUserSettingInt o => o.Validate(),
                    IUserSettingDouble o => o.Validate(),
                    _ => new ValidationResult(),
                };

                if (validation.Severity != ValidationResultLevel.Message)
                {
                    validations.Add(new ValidationResult(validation.Severity, validation.Message, userSetting.Name));
                }
            }
            return validations;
        }

        public List<ValidationResult> Validate(object rawSettings)
        {
            return Validate((TProfile)rawSettings);
        }

        /// <summary>
        /// Loads values from raw settings into a collection of user settings.
        /// </summary>
        public void LoadFromRaw(TProfile rawSettings, IEnumerable<IUserSetting> userSettings)
        {
            foreach (var setting in userSettings)
            {
                setting.LoadFromRaw(rawSettings);
            }
        }

        /// <summary>
        /// Loads values from raw settings into a collection of user settings.
        /// </summary>
        public void LoadFromRaw(object rawSettings, IEnumerable<IUserSetting> userSettings)
        {
            LoadFromRaw((TProfile)rawSettings, userSettings);
        }

        /// <summary>
        /// Loads values from collection of user settings into raw settings.
        /// </summary>
        public void ApplyToRaw(TProfile rawSettings, IEnumerable<IUserSettingGeneric<TProfile>> userSettings)
        {
            foreach (var setting in userSettings)
            {
                setting.ApplyToRaw(rawSettings);
            }
        }

        /// <summary>
        /// Loads values from collection of user settings into raw settings.
        /// </summary>
        public void ApplyToRaw(object rawSettings, IEnumerable<IUserSetting> userSettings)
        {
            ApplyToRaw((TProfile)rawSettings, userSettings);
        }

        public abstract void SetCulture(CultureInfo cultureInfo);
    }
}