using Sutro.Core.Settings.Part;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.Core.Translations;
using Sutro.PathWorks.Plugins.Core.UserSettings;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    [SuppressMessage("NDepend", "ND1000:AvoidTypesTooBig", Justification = "...")]
    [SuppressMessage("NDepend", "ND1002:AvoidTypesWithTooManyFields", Justification = "...")]
    public class PartUserSettingsFFF<TSettings> : UserSettingCollectionBase<TSettings> where TSettings : PartProfileFFF
    {
        #region Basic

        public static readonly UserSettingGroup GroupBasic =
            new UserSettingGroup(() => UserSettingTranslations.GroupBasic);

        public UserSettingString<TSettings> Name { get; } = new UserSettingString<TSettings>(
            "PrintUserSettingsFFF.Name",
            () => UserSettingTranslations.Name_Name,
            () => UserSettingTranslations.Name_Description,
            GroupBasic,
            (settings) => settings.Name,
            (settings, val) => settings.Name = val);

        public UserSettingBool<TSettings> EnableBridging { get; } = new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.EnableBridging",
            () => UserSettingTranslations.EnableBridging_Name,
            () => UserSettingTranslations.EnableBridging_Description,
            GroupBasic,
            (settings) => settings.EnableBridging,
            (settings, val) => settings.EnableBridging = val);

        public UserSettingInt<TSettings> FloorLayers { get; } = new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.FloorLayers",
            () => UserSettingTranslations.FloorLayers_Name,
            () => UserSettingTranslations.FloorLayers_Description,
            GroupBasic,
            (settings) => settings.FloorLayers,
            (settings, val) => settings.FloorLayers = val,
            unitsF: () => UserSettingTranslations.Units_Count,
            new NumericInfoInt() { Minimum = new NumericBound<int>(0, true) });

        public UserSettingBool<TSettings> GenerateSupport { get; } = new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.GenerateSupport",
            () => UserSettingTranslations.GenerateSupport_Name,
            () => UserSettingTranslations.GenerateSupport_Description,
            GroupBasic,
            (settings) => settings.GenerateSupport,
            (settings, val) => settings.GenerateSupport = val);

        public UserSettingDouble<TSettings> LayerHeightMM { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.LayerHeightMM",
            () => UserSettingTranslations.LayerHeightMM_Name,
            () => UserSettingTranslations.LayerHeightMM_Description,
            GroupBasic,
            (settings) => settings.LayerHeightMM,
            (settings, val) => settings.LayerHeightMM = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false), Increment = 0.05 },
            decimalDigits: 3);

        public UserSettingDouble<TSettings> MinExtrudeSpeed { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.MinExtrudeSpeed",
            () => UserSettingTranslations.MinExtrudeSpeed_Name,
            () => UserSettingTranslations.MinExtrudeSpeed_Description,
            GroupBasic,
            (settings) => settings.MinExtrudeSpeed,
            (settings, val) => settings.MinExtrudeSpeed = val,
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false), Increment = 5 },
            decimalDigits: 0);

        public UserSettingInt<TSettings> RoofLayers { get; } = new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.RoofLayers",
            () => UserSettingTranslations.RoofLayers_Name,
            () => UserSettingTranslations.RoofLayers_Description,
            GroupBasic,
            (settings) => settings.RoofLayers,
            (settings, val) => settings.RoofLayers = val,
            unitsF: () => UserSettingTranslations.Units_Count,
            new NumericInfoInt() { Minimum = new NumericBound<int>(0, true) });

        public UserSettingInt<TSettings> Shells { get; } = new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.Shells",
            () => UserSettingTranslations.Shells_Name,
            () => UserSettingTranslations.Shells_Description,
            GroupBasic,
            (settings) => settings.Shells,
            (settings, val) => settings.Shells = val,
            unitsF: () => UserSettingTranslations.Units_Count,
            new NumericInfoInt() { Minimum = new NumericBound<int>(0, true) });

        #endregion Basic

        #region Bridging

        public static readonly UserSettingGroup GroupBridging =
            new UserSettingGroup(() => UserSettingTranslations.GroupBridging);

        public UserSettingDouble<TSettings> BridgeExtrudeSpeedX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.BridgeExtrudeSpeedX",
            () => UserSettingTranslations.BridgeExtrudeSpeedX_Name,
            () => UserSettingTranslations.BridgeExtrudeSpeedX_Description,
            GroupBridging,
            (settings) => settings.BridgeExtrudeSpeedX,
            (settings, val) => settings.BridgeExtrudeSpeedX = val,
            convertToPercentage: true,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            decimalDigits: 2);

        public UserSettingDouble<TSettings> BridgeFillNozzleDiamStepX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.BridgeFillNozzleDiamStepX",
            () => UserSettingTranslations.BridgeFillNozzleDiamStepX_Name,
            () => UserSettingTranslations.BridgeFillNozzleDiamStepX_Description,
            GroupBridging,
            (settings) => settings.BridgeFillNozzleDiamStepX,
            (settings, val) => settings.BridgeFillNozzleDiamStepX = val,
            convertToPercentage: true,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            decimalDigits: 2);

        public UserSettingDouble<TSettings> BridgeVolumeScale { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.BridgeVolumeScale",
            () => UserSettingTranslations.BridgeVolumeScale_Name,
            () => UserSettingTranslations.BridgeVolumeScale_Description,
            GroupBridging,
            (settings) => settings.BridgeVolumeScale,
            (settings, val) => settings.BridgeVolumeScale = val,
            convertToPercentage: true,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            decimalDigits: 2);

        public UserSettingDouble<TSettings> MaxBridgeWidthMM { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.MaxBridgeWidthMM",
            () => UserSettingTranslations.MaxBridgeWidthMM_Name,
            () => UserSettingTranslations.MaxBridgeWidthMM_Description,
            GroupBridging,
            (settings) => settings.MaxBridgeWidthMM,
            (settings, val) => settings.MaxBridgeWidthMM = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, true) },
            decimalDigits: 1);

        #endregion Bridging

        #region FirstLayer

        public static readonly UserSettingGroup GroupFirstLayer =
            new UserSettingGroup(() => UserSettingTranslations.GroupFirstLayer);

        public UserSettingDouble<TSettings> CarefulExtrudeSpeed { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.CarefulExtrudeSpeed",
            () => UserSettingTranslations.CarefulExtrudeSpeed_Name,
            () => UserSettingTranslations.CarefulExtrudeSpeed_Description,
            GroupFirstLayer,
            (settings) => settings.CarefulExtrudeSpeed,
            (settings, val) => settings.CarefulExtrudeSpeed = val,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false), Increment = 5 },
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            decimalDigits: 0);

        public UserSettingDouble<TSettings> StartLayerHeightMM { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.StartLayerHeightMM",
            () => UserSettingTranslations.StartLayerHeightMM_Name,
            () => UserSettingTranslations.StartLayerHeightMM_Description,
            GroupFirstLayer,
            (settings) => settings.StartLayerHeightMM,
            (settings, val) => settings.StartLayerHeightMM = val,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            decimalDigits: 3);

        public UserSettingInt<TSettings> StartLayers { get; } = new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.StartLayers",
            () => UserSettingTranslations.StartLayers_Name,
            () => UserSettingTranslations.StartLayers_Description,
            GroupFirstLayer,
            (settings) => settings.StartLayers,
            (settings, val) => settings.StartLayers = val,
            unitsF: () => UserSettingTranslations.Units_Count,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(0, true) });

        #endregion FirstLayer

        #region Miscellaneous

        public static readonly UserSettingGroup GroupMiscellaneous =
            new UserSettingGroup(() => UserSettingTranslations.GroupMiscellaneous);

        public UserSettingDouble<TSettings> MinLayerTime { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.MinLayerTime",
            () => UserSettingTranslations.MinLayerTime_Name,
            () => UserSettingTranslations.MinLayerTime_Description,
            GroupMiscellaneous,
            (settings) => settings.MinLayerTime,
            (settings, val) => settings.MinLayerTime = val,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, true) },
            unitsF: () => UserSettingTranslations.Units_Seconds,
            decimalDigits: 0);

        #endregion Miscellaneous

        #region Perimeters

        public static readonly UserSettingGroup GroupPerimeters =
            new UserSettingGroup(() => UserSettingTranslations.GroupPerimeters);

        public UserSettingBool<TSettings> ClipSelfOverlaps { get; } = new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.ClipSelfOverlaps",
            () => UserSettingTranslations.ClipSelfOverlaps_Name,
            () => UserSettingTranslations.ClipSelfOverlaps_Description,
            GroupPerimeters,
            (settings) => settings.ClipSelfOverlaps,
            (settings, val) => settings.ClipSelfOverlaps = val);

        public UserSettingInt<TSettings> InteriorSolidRegionShells { get; } = new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.InteriorSolidRegionShells",
            () => UserSettingTranslations.InteriorSolidRegionShells_Name,
            () => UserSettingTranslations.InteriorSolidRegionShells_Description,
            GroupPerimeters,
            (settings) => settings.InteriorSolidRegionShells,
            (settings, val) => settings.InteriorSolidRegionShells = val,
            unitsF: () => UserSettingTranslations.Units_Count,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(0, true) });

        public UserSettingBool<TSettings> OuterShellLast { get; } = new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.OuterShellLast",
            () => UserSettingTranslations.OuterShellLast_Name,
            () => UserSettingTranslations.OuterShellLast_Description,
            GroupPerimeters,
            (settings) => settings.OuterShellLast,
            (settings, val) => settings.OuterShellLast = val);

        public UserSettingDouble<TSettings> SelfOverlapToleranceX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SelfOverlapToleranceX",
            () => UserSettingTranslations.SelfOverlapToleranceX_Name,
            () => UserSettingTranslations.SelfOverlapToleranceX_Description,
            GroupPerimeters,
            (settings) => settings.SelfOverlapToleranceX,
            (settings, val) => settings.SelfOverlapToleranceX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        public UserSettingDouble<TSettings> ShellsFillNozzleDiamStepX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.ShellsFillNozzleDiamStepX",
            () => UserSettingTranslations.ShellsFillNozzleDiamStepX_Name,
            () => UserSettingTranslations.ShellsFillNozzleDiamStepX_Description,
            GroupPerimeters,
            (settings) => settings.ShellsFillNozzleDiamStepX,
            (settings, val) => settings.ShellsFillNozzleDiamStepX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        public UserSettingBool<TSettings> ShellRandomizeStart { get; } = new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.ShellRandomizeStart",
            () => UserSettingTranslations.ShellRandomizeStart_Name,
            () => UserSettingTranslations.ShellRandomizeStart_Description,
            GroupPerimeters,
            (settings) => settings.ShellRandomizeStart,
            (settings, val) => settings.ShellRandomizeStart = val);

        public UserSettingBool<TSettings> ZipperAlignedToPoint { get; } = new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.ZipperAlignedToPoint",
            () => UserSettingTranslations.ZipperAlignedToPoint_Name,
            () => UserSettingTranslations.ZipperAlignedToPoint_Description,
            GroupPerimeters,
            (settings) => settings.ZipperAlignedToPoint,
            (settings, val) => settings.ZipperAlignedToPoint = val);

        public UserSettingDouble<TSettings> ZipperLocationX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.ZipperLocationX",
            () => UserSettingTranslations.ZipperLocationX_Name,
            () => UserSettingTranslations.ZipperLocationX_Description,
            GroupPerimeters,
            (settings) => settings.ZipperLocationX,
            (settings, val) => settings.ZipperLocationX = val,
            decimalDigits: 1);

        public UserSettingDouble<TSettings> ZipperLocationY { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.ZipperLocationY",
            () => UserSettingTranslations.ZipperLocationY_Name,
            () => UserSettingTranslations.ZipperLocationY_Description,
            GroupPerimeters,
            (settings) => settings.ZipperLocationY,
            (settings, val) => settings.ZipperLocationY = val,
            decimalDigits: 1);

        #endregion Perimeters

        #region SolidFill

        public static readonly UserSettingGroup GroupSolidFill =
            new UserSettingGroup(() => UserSettingTranslations.GroupSolidFill);

        public UserSettingDouble<TSettings> SolidFillBorderOverlapX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SolidFillBorderOverlapX",
            () => UserSettingTranslations.SolidFillBorderOverlapX_Name,
            () => UserSettingTranslations.SolidFillBorderOverlapX_Description,
            GroupSolidFill,
            (settings) => settings.SolidFillBorderOverlapX,
            (settings, val) => settings.SolidFillBorderOverlapX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        public UserSettingDouble<TSettings> SolidFillNozzleDiamStepX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SolidFillNozzleDiamStepX",
            () => UserSettingTranslations.SolidFillNozzleDiamStepX_Name,
            () => UserSettingTranslations.SolidFillNozzleDiamStepX_Description,
            GroupSolidFill,
            (settings) => settings.SolidFillNozzleDiamStepX,
            (settings, val) => settings.SolidFillNozzleDiamStepX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        public UserSettingDoubleListVariableLength<TSettings> SolidFillAngles { get; } = new UserSettingDoubleListVariableLength<TSettings>(
            "PrintUserSettingsFFF.SolidFillAngles",
            () => UserSettingTranslations.SolidFillAngles_Name,
            () => UserSettingTranslations.SolidFillAngles_Description,
            GroupSolidFill,
            (settings) => settings.InfillAngles,
            (settings, val) => settings.InfillAngles = val,
            unitsF: () => UserSettingTranslations.Units_Degrees,
            numericInfo: new NumericInfoDouble()
            { 
                Minimum = new NumericBound<double>(0, true),
                Maximum = new NumericBound<double>(360, true)
            },
            convertToPercentage: false,
            decimalDigits: 1);

        public UserSettingDoubleListFixedLength<TSettings> DummyAnglesFixed { get; } = new UserSettingDoubleListFixedLength<TSettings>(
            "DummyAngles",
            () => "Dummy Angles",
            () => "",
            GroupSolidFill,
            (settings) => new List<double>() { 0, 10, 20 },
            (settings, val) => { return; },
            3,
            unitsF: () => UserSettingTranslations.Units_Degrees,
            numericInfo: new NumericInfoDouble()
            {
                Minimum = new NumericBound<double>(0, true),
                Maximum = new NumericBound<double>(360, true)
            },
            convertToPercentage: false,
            decimalDigits: 1);

        #endregion SolidFill

        #region SparseFill

        public static readonly UserSettingGroup GroupSparseFill =
            new UserSettingGroup(() => UserSettingTranslations.GroupSparseFill);

        public UserSettingDouble<TSettings> SparseFillBorderOverlapX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SparseFillBorderOverlapX",
            () => UserSettingTranslations.SparseFillBorderOverlapX_Name,
            () => UserSettingTranslations.SparseFillBorderOverlapX_Description,
            GroupSparseFill,
            (settings) => settings.SparseFillBorderOverlapX,
            (settings, val) => settings.SparseFillBorderOverlapX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        public UserSettingDouble<TSettings> SparseLinearInfillStepX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SparseLinearInfillStepX",
            () => UserSettingTranslations.SparseLinearInfillStepX_Name,
            () => UserSettingTranslations.SparseLinearInfillStepX_Description,
            GroupSparseFill,
            (settings) => settings.SparseLinearInfillStepX,
            (settings, val) => settings.SparseLinearInfillStepX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        #endregion SparseFill

        #region Speeds

        public static readonly UserSettingGroup GroupSpeeds =
            new UserSettingGroup(() => UserSettingTranslations.GroupSpeeds);

        public UserSettingDouble<TSettings> OuterPerimeterSpeedX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.OuterPerimeterSpeedX",
            () => UserSettingTranslations.OuterPerimeterSpeedX_Name,
            () => UserSettingTranslations.OuterPerimeterSpeedX_Description,
            GroupSpeeds,
            (settings) => settings.OuterPerimeterSpeedX,
            (settings, val) => settings.OuterPerimeterSpeedX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        public UserSettingDouble<TSettings> RapidExtrudeSpeed { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.RapidExtrudeSpeed",
            () => UserSettingTranslations.RapidExtrudeSpeed_Name,
            () => UserSettingTranslations.RapidExtrudeSpeed_Description,
            GroupSpeeds,
            (settings) => settings.RapidExtrudeSpeed,
            (settings, val) => settings.RapidExtrudeSpeed = val,
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false), Increment = 5 },
            decimalDigits: 0);

        public UserSettingDouble<TSettings> RapidTravelSpeed { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.RapidTravelSpeed",
            () => UserSettingTranslations.RapidTravelSpeed_Name,
            () => UserSettingTranslations.RapidTravelSpeed_Description,
            GroupSpeeds,
            (settings) => settings.RapidTravelSpeed,
            (settings, val) => settings.RapidTravelSpeed = val,
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false), Increment = 5 },
            decimalDigits: 0);

        public UserSettingDouble<TSettings> ZTravelSpeed { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.ZTravelSpeed",
            () => UserSettingTranslations.ZTravelSpeed_Name,
            () => UserSettingTranslations.ZTravelSpeed_Description,
            GroupSpeeds,
            (settings) => settings.ZTravelSpeed,
            (settings, val) => settings.ZTravelSpeed = val,
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false), Increment = 5 },
            decimalDigits: 0);

        #endregion Speeds

        #region Support

        public static readonly UserSettingGroup GroupSupport =
            new UserSettingGroup(() => UserSettingTranslations.GroupSupport);

        public UserSettingBool<TSettings> EnableSupportReleaseOpt { get; } = new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.EnableSupportReleaseOpt",
            () => UserSettingTranslations.EnableSupportReleaseOpt_Name,
            () => UserSettingTranslations.EnableSupportReleaseOpt_Description,
            GroupSupport,
            (settings) => settings.EnableSupportReleaseOpt,
            (settings, val) => settings.EnableSupportReleaseOpt = val);

        public UserSettingBool<TSettings> EnableSupportShell { get; } = new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.EnableSupportShell",
            () => UserSettingTranslations.EnableSupportShell_Name,
            () => UserSettingTranslations.EnableSupportShell_Description,
            GroupSupport,
            (settings) => settings.EnableSupportShell,
            (settings, val) => settings.EnableSupportShell = val);

        public UserSettingDouble<TSettings> SupportAreaOffsetX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportAreaOffsetX",
            () => UserSettingTranslations.SupportAreaOffsetX_Name,
            () => UserSettingTranslations.SupportAreaOffsetX_Description,
            GroupSupport,
            (settings) => settings.SupportAreaOffsetX,
            (settings, val) => settings.SupportAreaOffsetX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            decimalDigits: 2);

        public UserSettingBool<TSettings> SupportMinZTips { get; } = new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.SupportMinZTips",
            () => UserSettingTranslations.SupportMinZTips_Name,
            () => UserSettingTranslations.SupportMinZTips_Description,
            GroupSupport,
            (settings) => settings.SupportMinZTips,
            (settings, val) => settings.SupportMinZTips = val);

        public UserSettingDouble<TSettings> SupportOverhangAngleDeg { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportOverhangAngleDeg",
            () => UserSettingTranslations.SupportOverhangAngleDeg_Name,
            () => UserSettingTranslations.SupportOverhangAngleDeg_Description,
            GroupSupport,
            (settings) => settings.SupportOverhangAngleDeg,
            (settings, val) => settings.SupportOverhangAngleDeg = val,
            unitsF: () => UserSettingTranslations.Units_Degrees,
            numericInfo: new NumericInfoDouble()
            {
                Minimum = new NumericBound<double>(0, false),
                Maximum = new NumericBound<double>(90, false),
                Increment = 5
            },
            decimalDigits: 1);

        public UserSettingDouble<TSettings> SupportPointDiam { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportPointDiam",
            () => UserSettingTranslations.SupportPointDiam_Name,
            () => UserSettingTranslations.SupportPointDiam_Description,
            GroupSupport,
            (settings) => settings.SupportPointDiam,
            (settings, val) => settings.SupportPointDiam = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            decimalDigits: 1);

        public UserSettingInt<TSettings> SupportPointSides { get; } = new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.SupportPointSides",
            () => UserSettingTranslations.SupportPointSides_Name,
            () => UserSettingTranslations.SupportPointSides_Description,
            GroupSupport,
            (settings) => settings.SupportPointSides,
            (settings, val) => settings.SupportPointSides = val,
            unitsF: () => UserSettingTranslations.Units_Count,
            numericInfo: new NumericInfoInt() { Minimum = new NumericBound<int>(3, true) });

        public UserSettingDouble<TSettings> SupportRegionJoinTolX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportRegionJoinTolX",
            () => UserSettingTranslations.SupportRegionJoinTolX_Name,
            () => UserSettingTranslations.SupportRegionJoinTolX_Description,
            GroupSupport,
            (settings) => settings.SupportRegionJoinTolX,
            (settings, val) => settings.SupportRegionJoinTolX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        public UserSettingDouble<TSettings> SupportReleaseGap { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportReleaseGap",
            () => UserSettingTranslations.SupportReleaseGap_Name,
            () => UserSettingTranslations.SupportReleaseGap_Description,
            GroupSupport,
            (settings) => settings.SupportReleaseGap,
            (settings, val) => settings.SupportReleaseGap = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, true) },
            decimalDigits: 3);

        public UserSettingDouble<TSettings> SupportSolidSpace { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportSolidSpace",
            () => UserSettingTranslations.SupportSolidSpace_Name,
            () => UserSettingTranslations.SupportSolidSpace_Description,
            GroupSupport,
            (settings) => settings.SupportSolidSpace,
            (settings, val) => settings.SupportSolidSpace = val,
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, true) },
            decimalDigits: 3);

        public UserSettingDouble<TSettings> SupportSpacingStepX { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportSpacingStepX",
            () => UserSettingTranslations.SupportSpacingStepX_Name,
            () => UserSettingTranslations.SupportSpacingStepX_Description,
            GroupSupport,
            (settings) => settings.SupportSpacingStepX,
            (settings, val) => settings.SupportSpacingStepX = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        public UserSettingDouble<TSettings> SupportVolumeScale { get; } = new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportVolumeScale",
            () => UserSettingTranslations.SupportVolumeScale_Name,
            () => UserSettingTranslations.SupportVolumeScale_Description,
            GroupSupport,
            (settings) => settings.SupportVolumeScale,
            (settings, val) => settings.SupportVolumeScale = val,
            unitsF: () => UserSettingTranslations.Units_Percentage,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false) },
            convertToPercentage: true,
            decimalDigits: 2);

        #endregion Support

        #region Retraction

        public static readonly UserSettingGroup GroupRetraction =
            new UserSettingGroup(() => UserSettingTranslations.GroupRetraction);

        public UserSettingDouble<TSettings> MinRetractTravelLength { get; } = new UserSettingDouble<TSettings>(
            "MaterialUserSettingsFFF.MinRetractTravelLength",
            () => UserSettingTranslations.MinRetractTravelLength_Name,
            () => UserSettingTranslations.MinRetractTravelLength_Description,
            GroupRetraction,
            (settings) => settings.MinRetractTravelLength,
            (settings, val) => settings.MinRetractTravelLength = val,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, true) },
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            decimalDigits: 1);

        public UserSettingDouble<TSettings> RetractDistanceMM { get; } = new UserSettingDouble<TSettings>(
            "MaterialUserSettingsFFF.RetractDistanceMM",
            () => UserSettingTranslations.RetractDistanceMM_Name,
            () => UserSettingTranslations.RetractDistanceMM_Description,
            GroupRetraction,
            (settings) => settings.RetractDistanceMM,
            (settings, val) => settings.RetractDistanceMM = val,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, true) },
            unitsF: () => UserSettingTranslations.Units_Millimeters,
            decimalDigits: 2);

        public UserSettingDouble<TSettings> RetractSpeed { get; } = new UserSettingDouble<TSettings>(
            "MaterialUserSettingsFFF.RetractSpeed",
            () => UserSettingTranslations.RetractSpeed_Name,
            () => UserSettingTranslations.RetractSpeed_Description,
            GroupRetraction,
            (settings) => settings.RetractSpeed,
            (settings, val) => settings.RetractSpeed = val,
            numericInfo: new NumericInfoDouble() { Minimum = new NumericBound<double>(0, false), Increment = 5 },
            unitsF: () => UserSettingTranslations.Units_MillimetersPerMinute,
            decimalDigits: 0);

        # endregion

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