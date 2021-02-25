using gs;
using Sutro.Core.Settings;
using Sutro.PathWorks.Plugins.API.Generators;
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
        public override ISettingsManager SettingsManager { get; } =
            new SettingsManagerFFF();

        public override List<IVisualizer> Visualizers { get; } = new List<IVisualizer>() {
            TubeVisualizerFFF.Create(),
        };

        public override IGenerator<PrintProfileFFF> Generator { get; } =
            new WrappedGenerator<SingleMaterialFFFPrintGenerator, PrintProfileFFF>(
                new PrintGeneratorManager<SingleMaterialFFFPrintGenerator, PrintProfileFFF>(
                    new PrintProfileFFF(), default, default, new ConsoleLogger(), true));

        public override string Name { get; } = "FFF";

        public override string Description { get; } = "Basic thermoplastic extrusion deposition";

        public override List<string> FileExtensions { get; } = new List<string>() { "gcode", "gco" };
    }
}