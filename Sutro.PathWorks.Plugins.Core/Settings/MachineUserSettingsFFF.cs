using Sutro.Core.Settings.Machine;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.Core.Translations;
using Sutro.PathWorks.Plugins.Core.UserSettings;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    [SuppressMessage("NDepend", "ND1000:AvoidTypesTooBig", Justification = "...")]
    [SuppressMessage("NDepend", "ND1002:AvoidTypesWithTooManyFields", Justification = "...")]
    public class MachineUserSettingsFFF<TSettings> : UserSettingCollectionBase<TSettings> where TSettings : MachineProfileFFF
    {
        #region Identifiers

        public static readonly UserSettingGroup GroupIdentifiers =
            new UserSettingGroup(() => UserSettingTranslations.GroupIdentifiers);

        public UserSettingString<TSettings> Name { get; } = new UserSettingString<TSettings>(
             "MachineUserSettingsFFF.Name",
             () => UserSettingTranslations.Name_Name,
             () => UserSettingTranslations.Name_Description,
             GroupIdentifiers,
             (settings) => settings.Name,
             (settings, val) => settings.Name = val);

        public UserSettingString<TSettings> ManufacturerName { get; } = new UserSettingString<TSettings>(
            "MachineUserSettingsFFF.ManufacturerName",
            () => UserSettingTranslations.ManufacturerName_Name,
            () => UserSettingTranslations.ManufacturerName_Description,
            GroupIdentifiers,
            (settings) => settings.ManufacturerName,
            (settings, val) => settings.ManufacturerName = val);

        public UserSettingString<TSettings> ModelIdentifier { get; } = new UserSettingString<TSettings>(
            "MachineUserSettingsFFF.ModelIdentifier",
            () => UserSettingTranslations.ModelIdentifier_Name,
            () => UserSettingTranslations.ModelIdentifier_Description,
            GroupIdentifiers,
            (settings) => settings.ModelIdentifier,
            (settings, val) => settings.ModelIdentifier = val);

        #endregion Identifiers

        #region Extruder

        public static readonly UserSettingGroup GroupExtruder =
            new UserSettingGroup(() => UserSettingTranslations.GroupExtruder);

        public UserSettingInt<TSettings> MaxExtruderTempC { get; } = new UserSettingInt<TSettings>(
            "MachineUserSettingsFFF.MaxExtruderTempC",
            () => UserSettingTranslations.MaxExtruderTempC_Name,
            () => UserSettingTranslations.MaxExtruderTempC_Description,
            GroupExtruder,
            (settings) => settings.MaxExtruderTempC,
            (settings, val) => settings.MaxExtruderTempC = val,
            unitsF: () => UserSettingTranslations.Units_DegreesCelsius,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(20, true) });

        public UserSettingInt<TSettings> MinExtruderTempC { get; } = new UserSettingInt<TSettings>(
            "MachineUserSettingsFFF.MinExtruderTempC",
            () => UserSettingTranslations.MinExtruderTempC_Name,
            () => UserSettingTranslations.MinExtruderTempC_Description,
            GroupExtruder,
            (settings) => settings.MinExtruderTempC,
            (settings, val) => settings.MinExtruderTempC = val,
            unitsF: () => UserSettingTranslations.Units_DegreesCelsius,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(20, true) });

        public UserSettingDouble<TSettings> NozzleDiamMM { get; } = new UserSettingDouble<TSettings>(
            "MachineUserSettingsFFF.NozzleDiamMM",
            () => UserSettingTranslations.NozzleDiamMM_Name,
            () => UserSettingTranslations.NozzleDiamMM_Description,
            GroupExtruder,
            (settings) => settings.NozzleDiamMM,
            (settings, val) => settings.NozzleDiamMM = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            decimalDigits:3);


        #endregion Extruder

        #region PrintVolume

        public static readonly UserSettingGroup GroupPrintVolume =
            new UserSettingGroup(() => UserSettingTranslations.GroupPrintVolume);

        public UserSettingDouble<TSettings> BedSizeXMM { get; } = new UserSettingDouble<TSettings>(
            "MachineUserSettingsFFF.BedSizeXMM",
            () => UserSettingTranslations.BedSizeXMM_Name,
            () => UserSettingTranslations.BedSizeXMM_Description,
            GroupPrintVolume,
            (settings) => settings.BedSizeXMM,
            (settings, val) => settings.BedSizeXMM = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(10, false), Increment = 5 },
            decimalDigits: 0);

        public UserSettingDouble<TSettings> BedSizeYMM { get; } = new UserSettingDouble<TSettings>(
            "MachineUserSettingsFFF.BedSizeYMM",
            () => UserSettingTranslations.BedSizeYMM_Name,
            () => UserSettingTranslations.BedSizeYMM_Description,
            GroupPrintVolume,
            (settings) => settings.BedSizeYMM,
            (settings, val) => settings.BedSizeYMM = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(10, false), Increment = 5 },
            decimalDigits: 0);
        public UserSettingDouble<TSettings> MaxHeightMM { get; } = new UserSettingDouble<TSettings>(
            "MachineUserSettingsFFF.MaxHeightMM",
            () => UserSettingTranslations.MaxHeightMM_Name,
            () => UserSettingTranslations.MaxHeightMM_Description,
            GroupPrintVolume,
            (settings) => settings.MaxHeightMM,
            (settings, val) => settings.MaxHeightMM = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(10, false), Increment = 5 },
            decimalDigits: 0);

        #endregion PrintVolume

        #region Speeds

        public static readonly UserSettingGroup GroupSpeeds =
            new UserSettingGroup(() => UserSettingTranslations.GroupSpeeds);

        public UserSettingInt<TSettings> MaxExtrudeSpeedMMM { get; } = new UserSettingInt<TSettings>(
            "MachineUserSettingsFFF.MaxExtrudeSpeedMMM",
            () => UserSettingTranslations.MaxExtrudeSpeedMMM_Name,
            () => UserSettingTranslations.MaxExtrudeSpeedMMM_Description,
            GroupSpeeds,
            (settings) => settings.MaxExtrudeSpeedMMM,
            (settings, val) => settings.MaxExtrudeSpeedMMM = val,
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(0, false), Increment = 10 } );


        public UserSettingInt<TSettings> MaxRetractSpeedMMM { get; } = new UserSettingInt<TSettings>(
            "MachineUserSettingsFFF.MaxRetractSpeedMMM",
            () => UserSettingTranslations.MaxRetractSpeedMMM_Name,
            () => UserSettingTranslations.MaxRetractSpeedMMM_Description,
            GroupSpeeds,
            (settings) => settings.MaxRetractSpeedMMM,
            (settings, val) => settings.MaxRetractSpeedMMM = val,
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(0, false), Increment = 10 });

        public UserSettingInt<TSettings> MaxTravelSpeedMMM { get; } = new UserSettingInt<TSettings>(
            "MachineUserSettingsFFF.MaxTravelSpeedMMM",
            () => UserSettingTranslations.MaxTravelSpeedMMM_Name,
            () => UserSettingTranslations.MaxTravelSpeedMMM_Description,
            GroupSpeeds,
            (settings) => settings.MaxTravelSpeedMMM,
            (settings, val) => settings.MaxTravelSpeedMMM = val,
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(0, false), Increment = 10 } );

        public UserSettingInt<TSettings> MaxZTravelSpeedMMM { get; } = new UserSettingInt<TSettings>(
            "MachineUserSettingsFFF.MaxZTravelSpeedMMM",
            () => UserSettingTranslations.MaxZTravelSpeedMMM_Name,
            () => UserSettingTranslations.MaxZTravelSpeedMMM_Description,
            GroupSpeeds,
            (settings) => settings.MaxZTravelSpeedMMM,
            (settings, val) => settings.MaxZTravelSpeedMMM = val,
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(0, false), Increment = 10 });

        #endregion Speeds

        #region Capabilities

        public static readonly UserSettingGroup GroupCapabilities =
            new UserSettingGroup(() => UserSettingTranslations.GroupCapabilities);

        public UserSettingBool<TSettings> HasAutoBedLeveling { get; } = new UserSettingBool<TSettings>(
            "MachineUserSettingsFFF.HasAutoBedLeveling",
            () => UserSettingTranslations.HasAutoBedLeveling_Name,
            () => UserSettingTranslations.HasAutoBedLeveling_Description,
            GroupCapabilities,
            (settings) => settings.HasAutoBedLeveling,
            (settings, val) => settings.HasAutoBedLeveling = val);

        public UserSettingBool<TSettings> HasHeatedBed { get; } = new UserSettingBool<TSettings>(
            "MachineUserSettingsFFF.HasHeatedBed",
            () => UserSettingTranslations.HasHeatedBed_Name,
            () => UserSettingTranslations.HasHeatedBed_Description,
            GroupCapabilities,
            (settings) => settings.HasHeatedBed,
            (settings, val) => settings.HasHeatedBed = val);

        public UserSettingDouble<TSettings> MaxLayerHeightMM { get; } = new UserSettingDouble<TSettings>(
            "MachineUserSettingsFFF.MaxLayerHeightMM",
            () => UserSettingTranslations.MaxLayerHeightMM_Name,
            () => UserSettingTranslations.MaxLayerHeightMM_Description,
            GroupCapabilities,
            (settings) => settings.MaxLayerHeightMM,
            (settings, val) => settings.MaxLayerHeightMM = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false), Increment = 0.05 },
            decimalDigits: 3);

        public UserSettingDouble<TSettings> MinLayerHeightMM { get; } = new UserSettingDouble<TSettings>(
            "MachineUserSettingsFFF.MinLayerHeightMM",
            () => UserSettingTranslations.MinLayerHeightMM_Name,
            () => UserSettingTranslations.MinLayerHeightMM_Description,
            GroupCapabilities,
            (settings) => settings.MinLayerHeightMM,
            (settings, val) => settings.MinLayerHeightMM = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false), Increment = 0.05 },
            decimalDigits: 3);

        #endregion Capabilities

        #region HeatedBed

        public static readonly UserSettingGroup GroupHeatedBed =
            new UserSettingGroup(() => UserSettingTranslations.GroupHeatedBed);

        public UserSettingInt<TSettings> MaxBedTempC { get; } = new UserSettingInt<TSettings>(
            "MachineUserSettingsFFF.MaxBedTempC",
            () => UserSettingTranslations.MaxBedTempC_Name,
            () => UserSettingTranslations.MaxBedTempC_Description,
            GroupHeatedBed,
            (settings) => settings.MaxBedTempC,
            (settings, val) => settings.MaxBedTempC = val,
            unitsF: () => UserSettingTranslations.Units_DegreesCelsius,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(0, true) });

        public UserSettingInt<TSettings> MinBedTempC { get; } = new UserSettingInt<TSettings>(
            "MachineUserSettingsFFF.MinBedTempC",
            () => UserSettingTranslations.MinBedTempC_Name,
            () => UserSettingTranslations.MinBedTempC_Description,
            GroupHeatedBed,
            (settings) => settings.MinBedTempC,
            (settings, val) => settings.MinBedTempC = val,
            unitsF: () => UserSettingTranslations.Units_DegreesCelsius,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(0, true) });

        #endregion HeatedBed

        #region Advanced

        public static readonly UserSettingGroup GroupAdvanced =
            new UserSettingGroup(() => UserSettingTranslations.GroupAdvanced);

        public UserSettingBool<TSettings> EnableAutoBedLeveling { get; } = new UserSettingBool<TSettings>(
            "MachineUserSettingsFFF.EnableAutoBedLeveling",
            () => UserSettingTranslations.EnableAutoBedLeveling_Name,
            () => UserSettingTranslations.EnableAutoBedLeveling_Description,
            GroupAdvanced,
            (settings) => settings.EnableAutoBedLeveling,
            (settings, val) => settings.EnableAutoBedLeveling = val);

        public UserSettingEnum<TSettings> Firmware { get; } = new UserSettingEnum<TSettings>(
            "Machine.Firmware",
            () => UserSettingTranslations.Machine_Firmware_Name,
            () => UserSettingTranslations.Machine_Firmware_Description,
            GroupAdvanced,
            (settings) => (int)settings.Firmware,
            (settings, val) => settings.Firmware = (FirmwareOptions)Enum.ToObject(typeof(FirmwareOptions), val),
            options: new List<UserSettingEnumOption>()
            {
                CreateEnumOption(FirmwareOptions.RepRap, () => "RepRap"),
                CreateEnumOption(FirmwareOptions.Prusa, () => "Smoothie"),
                CreateEnumOption(FirmwareOptions.Printrbot, () => "Duet (PB)"),
                CreateEnumOption(FirmwareOptions.Monoprice, () => "Duet (MP)"),
                CreateEnumOption(FirmwareOptions.Makerbot, () => "Sailfish"),
                CreateEnumOption(FirmwareOptions.Flashforge, () => "Duet (FF)"),
            }
        );

        private static UserSettingEnumOption CreateEnumOption<T> (T entry, Func<string> translation) where T : Enum
        {
            int typecode;
            checked
            {
                typecode = Convert.ToInt32(entry.GetTypeCode());
            }

            return new UserSettingEnumOption(typecode, entry.ToString(), translation);
        }

        #endregion Advanced

        /// <summary>
        /// Sets the culture for name & description strings.
        /// </summary>
        /// <remarks>
        /// Any translation resources used in derived classes should override to set the culture
        /// on the resource manager, while still calling the base method.
        /// </remarks>
        /// <param name="cultureInfo"></param>
        public override void SetCulture(CultureInfo cultureInfo)
        {
            UserSettingTranslations.Culture = cultureInfo;
        }
    }
}