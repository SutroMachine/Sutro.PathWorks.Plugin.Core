using g3;
using gs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sutro.Core.Models.GCode;
using Sutro.PathWorks.Plugins.Core.Visualizers;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Tests
{
    [TestClass]
    public class EngineFFFTests
    {
        [TestMethod]
        public void GCodeFromCube()
        {
            var gcode = CreateGCode();
            Assert.IsTrue(gcode.LineCount > 30);
        }

        [TestMethod]
        public void Visualizer()
        {
            var gcode = CreateGCode();

            var visualizer = new VolumetricBeadVisualizer();
            visualizer.BeginGCodeLineStream();
            foreach (var line in gcode.AllLines())
                visualizer.ProcessGCodeLine(line);
            visualizer.EndGCodeLineStream();
        }

        private static GCodeFile CreateGCode()
        {
            var engine = new Engines.EngineFFF();
            var boxGenerator = new TrivialBox3Generator();
            var mesh = boxGenerator.Generate().MakeDMesh();
            var parts = new List<Tuple<DMesh3, SingleMaterialFFFSettings>>();
            var settings = engine.SettingsManager.FactorySettings[0];
            var gcode = engine.Generator.GenerateGCode(parts, settings, out var generationReport);
            return gcode;
        }
    }
}