using Sutro.PathWorks.Plugins.API.Settings;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingString<TProfile> : UserSettingBase<TProfile, string>, IUserSettingString where TProfile : class
    {
        public List<Func<string, ValidationResult>> Validations { get; set; }

        public UserSettingString(
            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TProfile, string> loadF,
            Action<TProfile, string> applyF,
            IEnumerable<Func<string, ValidationResult>> validations = null,
            Func<string> unitsF = null) : base(id, nameF, descriptionF, group, loadF, applyF, unitsF)
        {
            Validations = new List<Func<string, ValidationResult>>();
            if (validations != null)
                Validations.AddRange(validations);
        }

        public ValidationResult Validate()
        {
            foreach (var validation in Validations)
            {
                var result = validation(Value);
                if (result.Severity != ValidationResultLevel.Message)
                {
                    return result;
                }
            }
            return new ValidationResult();
        }
    }
}