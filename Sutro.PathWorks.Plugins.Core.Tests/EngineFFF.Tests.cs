using g3;
using gs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sutro.Core.Models.GCode;
using Sutro.PathWorks.Plugins.Core.Visualizers;
using Sutro.PathWorks.Plugins.FFF;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Tests
{
    [TestClass]
    public class EngineFFFTests
    {
        private EngineFFF engine;

        [TestInitialize]
        public void Setup()
        {
            engine = new EngineFFF();
        }

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

            var visualizer = engine.Visualizers[0];
            visualizer.BeginGCodeLineStream();
            foreach (var line in gcode.AllLines())
                visualizer.ProcessGCodeLine(line);
            visualizer.EndGCodeLineStream();
        }

        private GCodeFile CreateGCode()
        {
            var boxGenerator = new TrivialBox3Generator();
            boxGenerator.Box = new Box3d(new Vector3d(0, 0, 5), new Vector3d(5, 5, 5));
            var mesh = boxGenerator.Generate().MakeDMesh();
            var parts = new List<Tuple<DMesh3, SingleMaterialFFFSettings>>();
            parts.Add(new Tuple<DMesh3, SingleMaterialFFFSettings>(mesh, null));
            var settings = engine.SettingsManager.FactorySettings[0];
            var gcode = engine.Generator.GenerateGCode(parts, settings, out var generationReport);
            return gcode;
        }
    }
}