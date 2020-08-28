using g3;
using gs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sutro.Core.Models.GCode;
using Sutro.PathWorks.Plugins.Core.Settings;
using Sutro.PathWorks.Plugins.Core.Visualizers;
using Sutro.PathWorks.Plugins.FFF;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

            int nRuns = 100;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < nRuns; i++)
            {
                var visualizer = engine.Visualizers[0];
                visualizer.BeginGCodeLineStream();
                foreach (var line in gcode.AllLines())
                    visualizer.ProcessGCodeLine(line);
                visualizer.EndGCodeLineStream();
            }
            stopwatch.Stop();

            Console.WriteLine($"Average of {stopwatch.ElapsedMilliseconds / nRuns} ms per visualizer run");
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

        [TestMethod]
        public void UserSettings_MachineFFF()
        {
            var settings = engine.SettingsManager.MachineUserSettings;
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        public void UserSettings_MaterialFFF()
        {
            var settings = engine.SettingsManager.MaterialUserSettings;
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        public void UserSettings_PrintFFF()
        {
            var settings = engine.SettingsManager.PrintUserSettings;
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        public void VisualizerText()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(engine.Visualizers[0].Name));
        }

        [TestMethod]
        public void VisualizerCustomDataDetails()
        {
            Assert.IsNotNull(engine.Visualizers[0].FillTypes);
            Assert.IsTrue(engine.Visualizers[0].FillTypes.Count > 0);
        }

        [TestMethod]
        public void SettingsJSONSerializeDeserialize()
        {
            // Arrange
            var settings = engine.SettingsManager.FactorySettings[0];
            settings.RapidExtrudeSpeed = 555;

            // Act
            var json = engine.SettingsManager.SerializeJSON(settings);
            var settingsDeserialized = engine.SettingsManager.DeserializeJSON(json);

            // Assert
            Assert.AreEqual(555, settingsDeserialized.RapidExtrudeSpeed);
        }

        [TestMethod]
        public void GetMultipleFileExtensions()
        {
            // Act
            var extensions = engine.FileExtensions;

            // Assert
            Assert.AreEqual(2, extensions.Count);
        }
    }
}