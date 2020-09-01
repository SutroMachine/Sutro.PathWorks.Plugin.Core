using g3;
using gs;
using Sutro.Core.FunctionalTest;
using Sutro.Core.Models.GCode;
using Sutro.Core.Settings;
using Sutro.PathWorks.Plugins.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Engines
{
    public class WrappedGenerator<TGenerator, TSettings> : IGenerator<TSettings>
        where TGenerator : IPrintGenerator<TSettings>, new()
        where TSettings : PrintProfileBase, new()
    {
        private readonly PrintGeneratorManager<TGenerator, TSettings> printGeneratorManager;

        public WrappedGenerator(PrintGeneratorManager<TGenerator, TSettings> printGeneratorManager)
        {
            this.printGeneratorManager = printGeneratorManager;
        }

        public bool AcceptsParts => printGeneratorManager.AcceptsParts;

        public bool AcceptsPartSettings => printGeneratorManager.AcceptsPartSettings;

        public Version Version => printGeneratorManager.PrintGeneratorAssemblyVersion;

        public GCodeFile GenerateGCode(IList<Tuple<DMesh3, TSettings>> parts, TSettings globalSettings, out IEnumerable<string> generationReport, Action<string> gcodeLineReadyF = null, Action<string> progressMessageF = null)
        {
            var meshes = parts.Select(p => p.Item1);
            return printGeneratorManager.GCodeFromMeshes(meshes, out generationReport, globalSettings);
        }

        public GCodeFile GenerateGCode(IList<Tuple<DMesh3, object>> parts, object globalSettings, out IEnumerable<string> generationReport, Action<string> gcodeLineReadyF = null, Action<string> progressMessageF = null)
        {
            var meshes = parts.Select(p => p.Item1);
            return printGeneratorManager.GCodeFromMeshes(meshes, out generationReport, (TSettings)globalSettings);
        }

        public GCodeFile LoadGCode(TextReader input)
        {
            return printGeneratorManager.LoadGCode(input);
        }

        public void SaveGCode(TextWriter output, GCodeFile file)
        {
            printGeneratorManager.SaveGCodeToFile(output, file);
        }
    }
}