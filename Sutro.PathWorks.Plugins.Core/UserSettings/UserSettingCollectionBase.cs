using Sutro.PathWorks.Plugins.API.Settings;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public abstract class UserSettingCollectionBase<TSettings> : IUserSettingCollection<TSettings>
    {
        /// <summary>
        /// Provides iteration through user settings typed with underlying raw settings type.
        /// </summary>
        public IEnumerable<UserSettingBase<TSettings>> Settings()
        {
            foreach (var setting in EnumerateProperties())
                yield return setting;

            foreach (var setting in EnumerateFields())
                yield return setting;
        }

        private IEnumerable<UserSettingBase<TSettings>> EnumerateProperties()
        {
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                if (typeof(UserSettingBase<TSettings>).IsAssignableFrom(property.PropertyType))
                {
                    var setting = (UserSettingBase<TSettings>)property.GetValue(this);
                    if (!setting.Hidden)
                        yield return setting;
                }
            }
        }

        private IEnumerable<UserSettingBase<TSettings>> EnumerateFields()
        {
            foreach (FieldInfo field in GetType().GetFields())
            {
                if (typeof(UserSettingBase<TSettings>).IsAssignableFrom(field.FieldType))
                {
                    var setting = (UserSettingBase<TSettings>)field.GetValue(this);
                    if (!setting.Hidden)
                        yield return setting;
                }
            }
        }

        /// <summary>
        /// Provides iteration through user settings without needing underlying raw settings type. 
        /// </summary>
        /// <remarks>
        /// Common version of generic method IUserSettingCollection<TSettings>.Settings()
        /// </remarks>
        IEnumerable<UserSettingBase> IUserSettingCollection.Settings()
        {
            foreach (var setting in Settings())
            {
                yield return setting;
            }
        }

        /// <summary>
        /// Checks the individual validations of each user setting.
        /// </summary>
        /// <remarks>
        /// This method can be overridden in derived classes to add validations
        /// between combinations of user settings in addition to the individual checks.
        /// </remarks>
        public virtual List<ValidationResult> Validate(TSettings settings)
        {
            var validations = new List<ValidationResult>();
            foreach (var userSetting in Settings())
            {
                userSetting.LoadFromRaw(settings);
                var validation = userSetting.Validation;

                if (validation.Severity != ValidationResultLevel.Message)
                {
                    validations.Add(new ValidationResult(validation.Severity, validation.Message, userSetting.Name));
                }
            }
            return validations;
        }

        public List<ValidationResult> Validate(object rawSettings)
        {
            return Validate((TSettings)rawSettings);
        }


        /// <summary>
        /// Loads values from raw settings into a collection of user settings.
        /// </summary>
        public void LoadFromRaw(TSettings rawSettings, IEnumerable<UserSettingBase<TSettings>> userSettings)
        {
            foreach (var setting in userSettings)
            {
                setting.LoadFromRaw(rawSettings);
            }
        }

        /// <summary>
        /// Loads values from raw settings into a collection of user settings.
        /// </summary>
        public void LoadFromRaw(object rawSettings, IEnumerable<UserSettingBase> userSettings)
        {
            var userSettingsTyped = new List<UserSettingBase<TSettings>>();
            foreach (var setting in userSettings)
                userSettingsTyped.Add((UserSettingBase<TSettings>)setting);
            LoadFromRaw((TSettings)rawSettings, userSettingsTyped);
        }

        /// <summary>
        /// Loads values from collection of user settings into raw settings.
        /// </summary>
        public void ApplyToRaw(TSettings rawSettings, IEnumerable<UserSettingBase<TSettings>> userSettings)
        {
            foreach (var setting in userSettings)
            {
                setting.ApplyToRaw(rawSettings);
            }
        }

        /// <summary>
        /// Loads values from collection of user settings into raw settings.
        /// </summary>
        public void ApplyToRaw(object rawSettings, IEnumerable<UserSettingBase> userSettings)
        {
            ApplyToRaw((TSettings)rawSettings, (IEnumerable<UserSettingBase<TSettings>>)userSettings);
        }

        public abstract void SetCulture(CultureInfo cultureInfo);
    }


}