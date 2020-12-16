using g3;
using gs;
using Sutro.Core.Models.GCode;
using Sutro.Core.Settings;
using Sutro.PathWorks.Plugins.API.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sutro.PathWorks.Plugins.Core.Engines
{
    public class WrappedGenerator<TGenerator, TSettings> : IGenerator<TSettings>
        where TGenerator : IPrintGenerator<TSettings>, new()
        where TSettings : class, IPrintProfileFFF, new()
    {
        private readonly PrintGeneratorManager<TGenerator, TSettings> printGeneratorManager;

        public WrappedGenerator(PrintGeneratorManager<TGenerator, TSettings> printGeneratorManager)
        {
            this.printGeneratorManager = printGeneratorManager;
        }

        public bool AcceptsParts => printGeneratorManager.AcceptsParts;

        public bool AcceptsPartSettings => printGeneratorManager.AcceptsPartSettings;

        public Version Version => printGeneratorManager.PrintGeneratorAssemblyVersion;

        public GenerationResultBase GenerateGCode(IList<Tuple<DMesh3, TSettings>> parts, TSettings globalSettings, CancellationToken? cancellationToken = null)
        {
            var meshes = parts.Select(p => p.Item1);
            try
            {
                var gcode = printGeneratorManager.GCodeFromMeshes(meshes, out var details, globalSettings, cancellationToken);
                return new GenerationResultSuccess(gcode, new GCodeInfo(details.MaterialUsageEstimate, details.PrintTimeEstimate), details.Warnings);
            }
            catch (Exception e)
            {
                return new GenerationResultFailure(e.Message);
            }
        }

        public GenerationResultBase GenerateGCode(IList<Tuple<DMesh3, object>> parts, object globalSettings, CancellationToken? cancellationToken = null)
        {
            var meshes = parts.Select(p => p.Item1);
            try
            {
                var globalSettingsTyped = (TSettings)globalSettings;
                var gcode = printGeneratorManager.GCodeFromMeshes(meshes, out var details, globalSettingsTyped, cancellationToken);
                return new GenerationResultSuccess(gcode, new GCodeInfo(details.MaterialUsageEstimate, details.PrintTimeEstimate), details.Warnings);
            }
            catch (Exception e)
            {
                return new GenerationResultFailure(e.Message);
            }
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