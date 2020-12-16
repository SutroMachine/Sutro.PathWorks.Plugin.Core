using Sutro.PathWorks.Plugins.API.Settings;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.UserSettings
{
    public class UserSettingDoubleListFixedLength<TProfile> : UserSettingBase<TProfile, List<double>>, IUserSettingDoubleListFixedLength where TProfile : class
    {
        public bool ConvertToPercentage { get; set; }

        public int DecimalDigits { get; set; }

        public NumericInfoDouble NumericInfo { get; set; }

        public int Count { get; set; }

        public UserSettingDoubleListFixedLength(
            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TProfile, List<double>> loadF,
            Action<TProfile, List<double>> applyF,
            int count, 
            Func<string> unitsF = null,
            NumericInfoDouble numericInfo = null,
            bool convertToPercentage = false,
            int decimalDigits = 2) :
            base(id, nameF, descriptionF, group, loadF, applyF, unitsF)
        {
            Count = count;
            NumericInfo = numericInfo;
            ConvertToPercentage = convertToPercentage;
            DecimalDigits = decimalDigits;
        }

        public ValidationResult Validate()
        {
            return new ValidationResult();
        }
    }

    public class UserSettingDoubleListVariableLength<TProfile> : UserSettingBase<TProfile, List<double>>, IUserSettingDoubleListVariableLength where TProfile : class
    {
        public bool ConvertToPercentage { get; set; }

        public int DecimalDigits { get; set; }

        public NumericInfoDouble NumericInfo { get; set; }

        public int MinimumCount { get; set; }

        public int MaximumCount { get; set; }

        public UserSettingDoubleListVariableLength(
            string id,
            Func<string> nameF,
            Func<string> descriptionF,
            UserSettingGroup group,
            Func<TProfile, List<double>> loadF,
            Action<TProfile, List<double>> applyF,
            int? minimumCount = null,
            int? maximumCount = null,
            Func<string> unitsF = null,
            NumericInfoDouble numericInfo = null,
            bool convertToPercentage = false,
            int decimalDigits = 2) :
            base(id, nameF, descriptionF, group, loadF, applyF, unitsF)
        {
            MinimumCount = minimumCount ?? 1;
            MaximumCount = maximumCount ?? 100;
            NumericInfo = numericInfo;
            ConvertToPercentage = convertToPercentage;
            DecimalDigits = decimalDigits;
        }

        public ValidationResult Validate()
        {
            return new ValidationResult();
        }
    }
}