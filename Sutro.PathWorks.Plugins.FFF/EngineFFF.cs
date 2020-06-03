using gs;
using Sutro.PathWorks.Plugins.API;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.API.Visualizers;
using Sutro.PathWorks.Plugins.Core.Engines;
using Sutro.PathWorks.Plugins.Core.Settings;
using Sutro.PathWorks.Plugins.Core.Visualizers;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.FFF
{
    public class EngineFFF : EngineBase<SingleMaterialFFFSettings>
    {
        public override ISettingsManager<SingleMaterialFFFSettings> SettingsManager =>
            new SettingsManagerFFF();

        public override List<IVisualizer> Visualizers => new List<IVisualizer>() {
            TubeVisualizerFFF.Create(),
        };

        public override IGenerator<SingleMaterialFFFSettings> Generator =>
            new WrappedGenerator<SingleMaterialFFFPrintGenerator, SingleMaterialFFFSettings>(
                new PrintGeneratorManager<SingleMaterialFFFPrintGenerator, SingleMaterialFFFSettings>(
                    new SingleMaterialFFFSettings(), default, default, new ConsoleLogger(), true));

        public override string Name => "FFF";

        public override string Description => "Basic thermoplastic extrusion deposition";
    }
}