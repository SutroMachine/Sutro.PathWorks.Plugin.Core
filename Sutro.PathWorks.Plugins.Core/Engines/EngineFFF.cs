﻿using gs;
using Sutro.PathWorks.Plugins.API;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.API.Visualizers;
using Sutro.PathWorks.Plugins.Core.Settings;
using Sutro.PathWorks.Plugins.Core.Visualizers;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Engines
{
    public class EngineFFF : EngineBase<SingleMaterialFFFSettings>
    {
        public override ISettingsManager<SingleMaterialFFFSettings> SettingsManager =>
            new SettingsManagerFFF();

        public override List<IVisualizer> Visualizers => new List<IVisualizer>() {
            new VolumetricBeadVisualizer(),
        };

        public override IGenerator<SingleMaterialFFFSettings> Generator =>
            new WrappedGenerator<SingleMaterialFFFPrintGenerator, SingleMaterialFFFSettings>(
                new PrintGeneratorManager<SingleMaterialFFFPrintGenerator, SingleMaterialFFFSettings>(
                    new SingleMaterialFFFSettings(), default, default, new ConsoleLogger(), true));

        public override string Name => "fff";

        public override string Description => "Provides access to the basic print generator included in gsCore.Can only create gcode for a single mesh with single material.";
    }
}