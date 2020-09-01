using gs;
using Sutro.Core.Settings;
using Sutro.PathWorks.Plugins.API;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.API.Visualizers;
using Sutro.PathWorks.Plugins.Core.Engines;
using Sutro.PathWorks.Plugins.Core.Settings;
using Sutro.PathWorks.Plugins.Core.Visualizers;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.FFF
{
    public class EngineFFF : EngineBase<PrintProfileFFF>
    {
        public override ISettingsManager SettingsManager =>
            new SettingsManagerFFF();

        public override List<IVisualizer> Visualizers => new List<IVisualizer>() {
            TubeVisualizerFFF.Create(),
        };

        public override IGenerator<PrintProfileFFF> Generator =>
            new WrappedGenerator<SingleMaterialFFFPrintGenerator, PrintProfileFFF>(
                new PrintGeneratorManager<SingleMaterialFFFPrintGenerator, PrintProfileFFF>(
                    new PrintProfileFFF(), default, default, new ConsoleLogger(), true));

        public override string Name => "FFF";

        public override string Description => "Basic thermoplastic extrusion deposition";

        public override List<string> FileExtensions => new List<string>() { "gcode", "gco" };
    }
}