using gs;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.Core.Translations;
using Sutro.PathWorks.Plugins.Core.UserSettings;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Sutro.PathWorks.Plugins.Core.Settings
{
    [SuppressMessage("NDepend", "ND1000:AvoidTypesTooBig", Justification = "...")]
    [SuppressMessage("NDepend", "ND1002:AvoidTypesWithTooManyFields", Justification = "...")]
    public class PrintUserSettingsFFF<TSettings> : UserSettingCollectionBase<TSettings> where TSettings : SingleMaterialFFFSettings
    {
        #region Advanced

        public static readonly UserSettingGroup GroupAdvanced =
            new UserSettingGroup(() => UserSettingTranslations.GroupAdvanced);

        public UserSettingBool<TSettings> EnableAutoBedLeveling => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.EnableAutoBedLeveling",
            () => UserSettingTranslations.EnableAutoBedLeveling_Name,
            () => UserSettingTranslations.EnableAutoBedLeveling_Description,
            GroupAdvanced,
            (settings) => settings.Machine.EnableAutoBedLeveling,
            (settings, val) => settings.Machine.EnableAutoBedLeveling = val);

        #endregion Advanced

        #region Basic

        public static readonly UserSettingGroup GroupBasic =
            new UserSettingGroup(() => UserSettingTranslations.GroupBasic);

        public UserSettingString<TSettings> Identifier => new UserSettingString<TSettings>(
            "PrintUserSettingsFFF.Identifier",
            () => UserSettingTranslations.Identifier_Name,
            () => UserSettingTranslations.Identifier_Description,
            GroupBasic,
            (settings) => settings.ProfileName,
            (settings, val) => settings.ProfileName = val);

        public UserSettingBool<TSettings> EnableBridging => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.EnableBridging",
            () => UserSettingTranslations.EnableBridging_Name,
            () => UserSettingTranslations.EnableBridging_Description,
            GroupBasic,
            (settings) => settings.EnableBridging,
            (settings, val) => settings.EnableBridging = val);

        public UserSettingInt<TSettings> FloorLayers => new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.FloorLayers",
            () => UserSettingTranslations.FloorLayers_Name,
            () => UserSettingTranslations.FloorLayers_Description,
            GroupBasic,
            (settings) => settings.FloorLayers,
            (settings, val) => settings.FloorLayers = val,
            UserSettingNumericValidations<int>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingBool<TSettings> GenerateSupport => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.GenerateSupport",
            () => UserSettingTranslations.GenerateSupport_Name,
            () => UserSettingTranslations.GenerateSupport_Description,
            GroupBasic,
            (settings) => settings.GenerateSupport,
            (settings, val) => settings.GenerateSupport = val);

        public UserSettingDouble<TSettings> LayerHeightMM => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.LayerHeightMM",
            () => UserSettingTranslations.LayerHeightMM_Name,
            () => UserSettingTranslations.LayerHeightMM_Description,
            GroupBasic,
            (settings) => settings.LayerHeightMM,
            (settings, val) => settings.LayerHeightMM = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> MinExtrudeSpeed => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.MinExtrudeSpeed",
            () => UserSettingTranslations.MinExtrudeSpeed_Name,
            () => UserSettingTranslations.MinExtrudeSpeed_Description,
            GroupBasic,
            (settings) => settings.MinExtrudeSpeed,
            (settings, val) => settings.MinExtrudeSpeed = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingInt<TSettings> RoofLayers => new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.RoofLayers",
            () => UserSettingTranslations.RoofLayers_Name,
            () => UserSettingTranslations.RoofLayers_Description,
            GroupBasic,
            (settings) => settings.RoofLayers,
            (settings, val) => settings.RoofLayers = val,
            UserSettingNumericValidations<int>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingInt<TSettings> Shells => new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.Shells",
            () => UserSettingTranslations.Shells_Name,
            () => UserSettingTranslations.Shells_Description,
            GroupBasic,
            (settings) => settings.Shells,
            (settings, val) => settings.Shells = val,
            UserSettingNumericValidations<int>.ValidateMin(0, ValidationResultLevel.Error));

        #endregion Basic

        #region Bridging

        public static readonly UserSettingGroup GroupBridging =
            new UserSettingGroup(() => UserSettingTranslations.GroupBridging);

        public UserSettingDouble<TSettings> BridgeExtrudeSpeedX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.BridgeExtrudeSpeedX",
            () => UserSettingTranslations.BridgeExtrudeSpeedX_Name,
            () => UserSettingTranslations.BridgeExtrudeSpeedX_Description,
            GroupBridging,
            (settings) => settings.BridgeExtrudeSpeedX,
            (settings, val) => settings.BridgeExtrudeSpeedX = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> BridgeFillNozzleDiamStepX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.BridgeFillNozzleDiamStepX",
            () => UserSettingTranslations.BridgeFillNozzleDiamStepX_Name,
            () => UserSettingTranslations.BridgeFillNozzleDiamStepX_Description,
            GroupBridging,
            (settings) => settings.BridgeFillNozzleDiamStepX,
            (settings, val) => settings.BridgeFillNozzleDiamStepX = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> BridgeVolumeScale => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.BridgeVolumeScale",
            () => UserSettingTranslations.BridgeVolumeScale_Name,
            () => UserSettingTranslations.BridgeVolumeScale_Description,
            GroupBridging,
            (settings) => settings.BridgeVolumeScale,
            (settings, val) => settings.BridgeVolumeScale = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> MaxBridgeWidthMM => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.MaxBridgeWidthMM",
            () => UserSettingTranslations.MaxBridgeWidthMM_Name,
            () => UserSettingTranslations.MaxBridgeWidthMM_Description,
            GroupBridging,
            (settings) => settings.MaxBridgeWidthMM,
            (settings, val) => settings.MaxBridgeWidthMM = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        #endregion Bridging

        #region FirstLayer

        public static readonly UserSettingGroup GroupFirstLayer =
            new UserSettingGroup(() => UserSettingTranslations.GroupFirstLayer);

        public UserSettingDouble<TSettings> CarefulExtrudeSpeed => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.CarefulExtrudeSpeed",
            () => UserSettingTranslations.CarefulExtrudeSpeed_Name,
            () => UserSettingTranslations.CarefulExtrudeSpeed_Description,
            GroupFirstLayer,
            (settings) => settings.CarefulExtrudeSpeed,
            (settings, val) => settings.CarefulExtrudeSpeed = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> StartLayerHeightMM => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.StartLayerHeightMM",
            () => UserSettingTranslations.StartLayerHeightMM_Name,
            () => UserSettingTranslations.StartLayerHeightMM_Description,
            GroupFirstLayer,
            (settings) => settings.StartLayerHeightMM,
            (settings, val) => settings.StartLayerHeightMM = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingInt<TSettings> StartLayers => new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.StartLayers",
            () => UserSettingTranslations.StartLayers_Name,
            () => UserSettingTranslations.StartLayers_Description,
            GroupFirstLayer,
            (settings) => settings.StartLayers,
            (settings, val) => settings.StartLayers = val,
            UserSettingNumericValidations<int>.ValidateMin(0, ValidationResultLevel.Error));

        #endregion FirstLayer

        #region Miscellaneous

        public static readonly UserSettingGroup GroupMiscellaneous =
            new UserSettingGroup(() => UserSettingTranslations.GroupMiscellaneous);

        public UserSettingDouble<TSettings> MinLayerTime => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.MinLayerTime",
            () => UserSettingTranslations.MinLayerTime_Name,
            () => UserSettingTranslations.MinLayerTime_Description,
            GroupMiscellaneous,
            (settings) => settings.MinLayerTime,
            (settings, val) => settings.MinLayerTime = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        #endregion Miscellaneous

        #region Perimeters

        public static readonly UserSettingGroup GroupPerimeters =
            new UserSettingGroup(() => UserSettingTranslations.GroupPerimeters);

        public UserSettingBool<TSettings> ClipSelfOverlaps => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.ClipSelfOverlaps",
            () => UserSettingTranslations.ClipSelfOverlaps_Name,
            () => UserSettingTranslations.ClipSelfOverlaps_Description,
            GroupPerimeters,
            (settings) => settings.ClipSelfOverlaps,
            (settings, val) => settings.ClipSelfOverlaps = val);

        public UserSettingInt<TSettings> InteriorSolidRegionShells => new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.InteriorSolidRegionShells",
            () => UserSettingTranslations.InteriorSolidRegionShells_Name,
            () => UserSettingTranslations.InteriorSolidRegionShells_Description,
            GroupPerimeters,
            (settings) => settings.InteriorSolidRegionShells,
            (settings, val) => settings.InteriorSolidRegionShells = val,
            UserSettingNumericValidations<int>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingBool<TSettings> OuterShellLast => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.OuterShellLast",
            () => UserSettingTranslations.OuterShellLast_Name,
            () => UserSettingTranslations.OuterShellLast_Description,
            GroupPerimeters,
            (settings) => settings.OuterShellLast,
            (settings, val) => settings.OuterShellLast = val);

        public UserSettingDouble<TSettings> SelfOverlapToleranceX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SelfOverlapToleranceX",
            () => UserSettingTranslations.SelfOverlapToleranceX_Name,
            () => UserSettingTranslations.SelfOverlapToleranceX_Description,
            GroupPerimeters,
            (settings) => settings.SelfOverlapToleranceX,
            (settings, val) => settings.SelfOverlapToleranceX = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> ShellsFillNozzleDiamStepX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.ShellsFillNozzleDiamStepX",
            () => UserSettingTranslations.ShellsFillNozzleDiamStepX_Name,
            () => UserSettingTranslations.ShellsFillNozzleDiamStepX_Description,
            GroupPerimeters,
            (settings) => settings.ShellsFillNozzleDiamStepX,
            (settings, val) => settings.ShellsFillNozzleDiamStepX = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingBool<TSettings> ShellRandomizeStart => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.ShellRandomizeStart",
            () => UserSettingTranslations.ShellRandomizeStart_Name,
            () => UserSettingTranslations.ShellRandomizeStart_Description,
            GroupPerimeters,
            (settings) => settings.ShellRandomizeStart,
            (settings, val) => settings.ShellRandomizeStart = val);

        public UserSettingBool<TSettings> ZipperAlignedToPoint => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.ZipperAlignedToPoint",
            () => UserSettingTranslations.ZipperAlignedToPoint_Name,
            () => UserSettingTranslations.ZipperAlignedToPoint_Description,
            GroupPerimeters,
            (settings) => settings.ZipperAlignedToPoint,
            (settings, val) => settings.ZipperAlignedToPoint = val);

        public UserSettingDouble<TSettings> ZipperLocationX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.ZipperLocationX",
            () => UserSettingTranslations.ZipperLocationX_Name,
            () => UserSettingTranslations.ZipperLocationX_Description,
            GroupPerimeters,
            (settings) => settings.ZipperLocationX,
            (settings, val) => settings.ZipperLocationX = val);

        public UserSettingDouble<TSettings> ZipperLocationY => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.ZipperLocationY",
            () => UserSettingTranslations.ZipperLocationY_Name,
            () => UserSettingTranslations.ZipperLocationY_Description,
            GroupPerimeters,
            (settings) => settings.ZipperLocationY,
            (settings, val) => settings.ZipperLocationY = val);

        #endregion Perimeters

        #region SolidFill

        public static readonly UserSettingGroup GroupSolidFill =
            new UserSettingGroup(() => UserSettingTranslations.GroupSolidFill);

        public UserSettingDouble<TSettings> SolidFillBorderOverlapX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SolidFillBorderOverlapX",
            () => UserSettingTranslations.SolidFillBorderOverlapX_Name,
            () => UserSettingTranslations.SolidFillBorderOverlapX_Description,
            GroupSolidFill,
            (settings) => settings.SolidFillBorderOverlapX,
            (settings, val) => settings.SolidFillBorderOverlapX = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> SolidFillNozzleDiamStepX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SolidFillNozzleDiamStepX",
            () => UserSettingTranslations.SolidFillNozzleDiamStepX_Name,
            () => UserSettingTranslations.SolidFillNozzleDiamStepX_Description,
            GroupSolidFill,
            (settings) => settings.SolidFillNozzleDiamStepX,
            (settings, val) => settings.SolidFillNozzleDiamStepX = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        #endregion SolidFill

        #region SparseFill

        public static readonly UserSettingGroup GroupSparseFill =
            new UserSettingGroup(() => UserSettingTranslations.GroupSparseFill);

        public UserSettingDouble<TSettings> SparseFillBorderOverlapX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SparseFillBorderOverlapX",
           () => UserSettingTranslations.SparseFillBorderOverlapX_Name,
           () => UserSettingTranslations.SparseFillBorderOverlapX_Description,
           GroupSparseFill,
           (settings) => settings.SparseFillBorderOverlapX,
           (settings, val) => settings.SparseFillBorderOverlapX = val,
           UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> SparseLinearInfillStepX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SparseLinearInfillStepX",
           () => UserSettingTranslations.SparseLinearInfillStepX_Name,
           () => UserSettingTranslations.SparseLinearInfillStepX_Description,
           GroupSparseFill,
           (settings) => settings.SparseLinearInfillStepX,
           (settings, val) => settings.SparseLinearInfillStepX = val,
           UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        #endregion SparseFill

        #region Speeds

        public static readonly UserSettingGroup GroupSpeeds =
            new UserSettingGroup(() => UserSettingTranslations.GroupSpeeds);

        public UserSettingDouble<TSettings> OuterPerimeterSpeedX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.OuterPerimeterSpeedX",
            () => UserSettingTranslations.OuterPerimeterSpeedX_Name,
            () => UserSettingTranslations.OuterPerimeterSpeedX_Description,
            GroupSpeeds,
            (settings) => settings.OuterPerimeterSpeedX,
            (settings, val) => settings.OuterPerimeterSpeedX = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> RapidExtrudeSpeed => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.RapidExtrudeSpeed",
            () => UserSettingTranslations.RapidExtrudeSpeed_Name,
            () => UserSettingTranslations.RapidExtrudeSpeed_Description,
            GroupSpeeds,
            (settings) => settings.RapidExtrudeSpeed,
            (settings, val) => settings.RapidExtrudeSpeed = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> RapidTravelSpeed => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.RapidTravelSpeed",
            () => UserSettingTranslations.RapidTravelSpeed_Name,
            () => UserSettingTranslations.RapidTravelSpeed_Description,
            GroupSpeeds,
            (settings) => settings.RapidTravelSpeed,
            (settings, val) => settings.RapidTravelSpeed = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> ZTravelSpeed => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.ZTravelSpeed",
            () => UserSettingTranslations.ZTravelSpeed_Name,
            () => UserSettingTranslations.ZTravelSpeed_Description,
            GroupSpeeds,
            (settings) => settings.ZTravelSpeed,
            (settings, val) => settings.ZTravelSpeed = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        #endregion Speeds

        #region Support

        public static readonly UserSettingGroup GroupSupport =
            new UserSettingGroup(() => UserSettingTranslations.GroupSupport);

        public UserSettingBool<TSettings> EnableSupportReleaseOpt => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.EnableSupportReleaseOpt",
            () => UserSettingTranslations.EnableSupportReleaseOpt_Name,
            () => UserSettingTranslations.EnableSupportReleaseOpt_Description,
            GroupSupport,
            (settings) => settings.EnableSupportReleaseOpt,
            (settings, val) => settings.EnableSupportReleaseOpt = val);

        public UserSettingBool<TSettings> EnableSupportShell => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.EnableSupportShell",
            () => UserSettingTranslations.EnableSupportShell_Name,
            () => UserSettingTranslations.EnableSupportShell_Description,
            GroupSupport,
            (settings) => settings.EnableSupportShell,
            (settings, val) => settings.EnableSupportShell = val);

        public UserSettingDouble<TSettings> SupportAreaOffsetX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportAreaOffsetX",
            () => UserSettingTranslations.SupportAreaOffsetX_Name,
            () => UserSettingTranslations.SupportAreaOffsetX_Description,
            GroupSupport,
            (settings) => settings.SupportAreaOffsetX,
            (settings, val) => settings.SupportAreaOffsetX = val);

        public UserSettingBool<TSettings> SupportMinZTips => new UserSettingBool<TSettings>(
            "PrintUserSettingsFFF.SupportMinZTips",
            () => UserSettingTranslations.SupportMinZTips_Name,
            () => UserSettingTranslations.SupportMinZTips_Description,
            GroupSupport,
            (settings) => settings.SupportMinZTips,
            (settings, val) => settings.SupportMinZTips = val);

        public UserSettingDouble<TSettings> SupportOverhangAngleDeg => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportOverhangAngleDeg",
            () => UserSettingTranslations.SupportOverhangAngleDeg_Name,
            () => UserSettingTranslations.SupportOverhangAngleDeg_Description,
            GroupSupport,
            (settings) => settings.SupportOverhangAngleDeg,
            (settings, val) => settings.SupportOverhangAngleDeg = val,
            UserSettingNumericValidations<double>.ValidateMinMax(0, 90, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> SupportPointDiam => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportPointDiam",
            () => UserSettingTranslations.SupportPointDiam_Name,
            () => UserSettingTranslations.SupportPointDiam_Description,
            GroupSupport,
            (settings) => settings.SupportPointDiam,
            (settings, val) => settings.SupportPointDiam = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingInt<TSettings> SupportPointSides => new UserSettingInt<TSettings>(
            "PrintUserSettingsFFF.SupportPointSides",
            () => UserSettingTranslations.SupportPointSides_Name,
            () => UserSettingTranslations.SupportPointSides_Description,
            GroupSupport,
            (settings) => settings.SupportPointSides,
            (settings, val) => settings.SupportPointSides = val,
            UserSettingNumericValidations<int>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> SupportRegionJoinTolX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportRegionJoinTolX",
            () => UserSettingTranslations.SupportRegionJoinTolX_Name,
            () => UserSettingTranslations.SupportRegionJoinTolX_Description,
            GroupSupport,
            (settings) => settings.SupportRegionJoinTolX,
            (settings, val) => settings.SupportRegionJoinTolX = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> SupportReleaseGap => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportReleaseGap",
            () => UserSettingTranslations.SupportReleaseGap_Name,
            () => UserSettingTranslations.SupportReleaseGap_Description,
            GroupSupport,
            (settings) => settings.SupportReleaseGap,
            (settings, val) => settings.SupportReleaseGap = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> SupportSolidSpace => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportSolidSpace",
            () => UserSettingTranslations.SupportSolidSpace_Name,
            () => UserSettingTranslations.SupportSolidSpace_Description,
            GroupSupport,
            (settings) => settings.SupportSolidSpace,
            (settings, val) => settings.SupportSolidSpace = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> SupportSpacingStepX => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportSpacingStepX",
            () => UserSettingTranslations.SupportSpacingStepX_Name,
            () => UserSettingTranslations.SupportSpacingStepX_Description,
            GroupSupport,
            (settings) => settings.SupportSpacingStepX,
            (settings, val) => settings.SupportSpacingStepX = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        public UserSettingDouble<TSettings> SupportVolumeScale => new UserSettingDouble<TSettings>(
            "PrintUserSettingsFFF.SupportVolumeScale",
            () => UserSettingTranslations.SupportVolumeScale_Name,
            () => UserSettingTranslations.SupportVolumeScale_Description,
            GroupSupport,
            (settings) => settings.SupportVolumeScale,
            (settings, val) => settings.SupportVolumeScale = val,
            UserSettingNumericValidations<double>.ValidateMin(0, ValidationResultLevel.Error));

        #endregion Support

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