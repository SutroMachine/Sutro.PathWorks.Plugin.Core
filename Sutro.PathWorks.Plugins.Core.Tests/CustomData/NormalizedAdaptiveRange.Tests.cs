using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sutro.PathWorks.Plugins.Core.CustomData;

namespace Sutro.PathWorks.Plugins.Core.Tests.CustomData
{
    [TestClass]
    public class NormalizedAdaptiveRangeTests
    {
        [TestMethod]
        public void FormatColorScaleLabel()
        {
            // Arrange
            var range = CreateNormalizedAdaptiveRange();

            // Act
            range.ObserveValue(-10);
            range.ObserveValue(90);
            var label = range.FormatColorScaleLabel(30);

            // Assert
            var expected = $"{0.4:P2}";
            Assert.AreEqual(expected, label);
        }

        [TestMethod]
        public void RangeMinMax()
        {
            // Arrange
            var range = CreateNormalizedAdaptiveRange();

            // Act
            range.ObserveValue(19);
            range.ObserveValue(-10);
            range.ObserveValue(90);
            range.ObserveValue(50);

            // Assert
            Assert.AreEqual(-10, range.RangeMin);
            Assert.AreEqual(90, range.RangeMax);
        }

        [TestMethod]
        public void Label()
        {
            // Arrange
            var range = CreateNormalizedAdaptiveRange();

            // Assert
            Assert.AreEqual("Mock", range.Label);
        }

        private static NormalizedAdaptiveRange CreateNormalizedAdaptiveRange()
        {
            return new NormalizedAdaptiveRange(() => "Mock", (value) => $"{value:P2}");
        }
    }
}