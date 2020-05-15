﻿using Sutro.PathWorks.Plugins.API.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingEnum<TSettings> : UserSetting<TSettings, string>
    {
        private readonly Func<IList<Tuple<int, string, string>>> tupleF;
        public IList<Tuple<int, string, string>> Tuples => tupleF();

        /// <param name="tupleF">Tuples of (enum value, name, description), in that order.</param>
        public UserSettingEnum(
            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TSettings, string> loadF,
            Action<TSettings, string> applyF,
            Func<IList<Tuple<int, string, string>>> tupleF = null) : base(id, nameF, descriptionF, group, loadF, applyF, UserSettingEnumValidations.ValidateContains(tupleF, ValidationResultLevel.Error))
        {
            this.tupleF = tupleF;
        }
    }

    public static class UserSettingEnumValidations
    {
        public static Func<string, ValidationResult> ValidateContains(Func<IList<Tuple<int, string, string>>> tupleF, ValidationResultLevel level)
        {
            return (val) =>
            {
                var tuples = tupleF();
                foreach (var tuple in tuples)
                {
                    if (tuple.Item2 == val)
                        return new ValidationResult();
                }
                return new ValidationResult(level, string.Format("Must be one of {0}", string.Join(", ", tuples.Select(tuple => tuple.Item2).ToArray())));
            };
        }
    }
}