using g3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sutro.Core.Models.GCode;
using Sutro.Core.Settings;
using Sutro.Core.Settings.Machine;
using Sutro.Core.Settings.Material;
using Sutro.Core.Settings.Part;
using Sutro.PathWorks.Plugins.API.Generators;
using Sutro.PathWorks.Plugins.FFF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            var parts = new List<Tuple<DMesh3, PrintProfileFFF>>();
            parts.Add(new Tuple<DMesh3, PrintProfileFFF>(mesh, null));
            var settings = (PrintProfileFFF)engine.SettingsManager.CreateSettingsInstance();
            settings.Part.LayerHeightMM = 1;
            var result = engine.Generator.GenerateGCode(parts, settings) as GenerationResultSuccess;
            return result.File;
        }

        [TestMethod]
        public void ProfileManager_Machine_FactoryProfiles()
        {
            var settings = engine.SettingsManager.MachineProfileManager.FactoryProfiles[0];
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        public void ProfileManager_Machine_UserSettings()
        {
            var userSettings = engine.SettingsManager.MachineProfileManager.UserSettings;
            Assert.IsNotNull(userSettings);
        }

        [TestMethod]
        public void UserSettings_MachineFFF()
        {
            var settings = engine.SettingsManager.MachineProfileManager.FactoryProfiles[0];
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        public void UserSettings_MaterialFFF()
        {
            var settings = engine.SettingsManager.MaterialProfileManager.FactoryProfiles[0];
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        public void UserSettings_PartFFF()
        {
            var settings = engine.SettingsManager.PartProfileManager.FactoryProfiles[0];
            Assert.IsNotNull(settings);
        }

        [TestMethod]
        public void UserSettings_NotNullOrEmpty()
        {
            var userSettings = engine.SettingsManager.MachineProfileManager.UserSettings;
            Assert.IsNotNull(userSettings);

            var userSettingsList = userSettings.Settings().ToList();
            Assert.AreNotEqual(0, userSettingsList.Count);
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
            var settings = (PartProfileFFF)engine.SettingsManager.PartProfileManager.FactoryProfiles[0];
            settings.RapidExtrudeSpeed = 555;

            // Act
            var json = engine.SettingsManager.PartProfileManager.SerializeJSON(settings);
            var settingsDeserialized = (PartProfileFFF)engine.SettingsManager.PartProfileManager.DeserializeJSON(json);

            // Assert
            Assert.AreEqual(555, settingsDeserialized.RapidExtrudeSpeed, 1e-6);
        }

        [TestMethod]
        public void PartSettingsApplyJson()
        {
            // Arrange
            var settings = (PartProfileFFF)engine.SettingsManager.PartProfileManager.FactoryProfiles[0];

            // Act
            engine.SettingsManager.PartProfileManager.ApplyJSON(settings, "{\"RapidExtrudeSpeed\":555}");

            // Assert
            Assert.AreEqual(555, settings.RapidExtrudeSpeed);
        }

        [TestMethod]
        public void MachineSettingsApplyJson()
        {
            // Arrange
            var settings = (MachineProfileFFF)engine.SettingsManager.MachineProfileManager.FactoryProfiles[0];

            // Act
            engine.SettingsManager.MachineProfileManager.ApplyJSON(settings, "{\"MaxExtruderTempC\":555}");

            // Assert
            Assert.AreEqual(555, settings.MaxExtruderTempC = 555);
        }

        [TestMethod]
        public void MaterialSettingsApplyJson()
        {
            // Arrange
            var settings = (MaterialProfileFFF)engine.SettingsManager.MaterialProfileManager.FactoryProfiles[0];

            // Act
            engine.SettingsManager.MaterialProfileManager.ApplyJSON(settings, "{\"CostPerKG\":555}");

            // Assert
            Assert.AreEqual(555, settings.CostPerKG = 555);
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