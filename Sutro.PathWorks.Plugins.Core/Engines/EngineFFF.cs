using gs;
using Sutro.Core.Models.GCode;
using Sutro.PathWorks.Plugins.API;
using Sutro.PathWorks.Plugins.Core.Settings;
using Sutro.PathWorks.Plugins.Core.Visualizers;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Sutro.PathWorks.Plugins.Core.Engines
{
    [Export(typeof(IEngine))]
    [ExportMetadata("Name", "fff")]
    [ExportMetadata("Description", "Provides access to the basic print generator included in gsCore. Can only create gcode for a single mesh with single material.")]
    public class EngineFFF : Engine<SingleMaterialFFFSettings>
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

    }
}
