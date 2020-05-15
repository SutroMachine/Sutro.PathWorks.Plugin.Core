using g3;
using gs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var engine = new Engines.EngineFFF();

            var boxGenerator = new TrivialBox3Generator();
            var mesh = boxGenerator.Generate().MakeDMesh();
            var parts = new List<Tuple<DMesh3, SingleMaterialFFFSettings>>();
            var settings = engine.SettingsManager.FactorySettings[0];
            var gcode = engine.Generator.GenerateGCode(parts, settings, out var generationReport);

            Assert.IsTrue(gcode.LineCount > 30);
        }
    }
}