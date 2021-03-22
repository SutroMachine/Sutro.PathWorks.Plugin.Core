using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sutro.PathWorks.Plugins.API.Settings;
using Sutro.PathWorks.Plugins.Core.UserSettings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sutro.PathWorks.Plugins.Core.Tests
{
    [TestClass]
    public class NumericValidations_Tests
    {

        [TestMethod]
        public void Valid_NullNumericInfo()
        {
            NumericInfoInt info = null;
            var result = NumericValidation.Validate(info, 3);
            Assert.AreEqual(ValidationResultLevel.Message, result.Severity);
        }

        [TestMethod]
        public void Valid_NullMinimum()
        {
            NumericInfoInt info = new NumericInfoInt() { Maximum = new NumericBound<int>(10, true) };
            var result = NumericValidation.Validate(info, 10);
            Assert.AreEqual(ValidationResultLevel.Message, result.Severity);
        }

        [TestMethod]
        public void Valid_NullMaximum()
        {
            NumericInfoInt info = new NumericInfoInt() { Minimum = new NumericBound<int>(0, true) };
            var result = NumericValidation.Validate(info, 0);
            Assert.AreEqual(ValidationResultLevel.Message, result.Severity);
        }

        [TestMethod]
        public void Valid()
        {
            NumericInfoInt info = new NumericInfoInt() { 
                Minimum = new NumericBound<int>(0, true),
                Maximum = new NumericBound<int>(10, true)
            };
            var result = NumericValidation.Validate(info, 0);
            Assert.AreEqual(ValidationResultLevel.Message, result.Severity);
        }

        [TestMethod]
        public void Invalid_OutOfRange()
        {
            NumericInfoInt info = new NumericInfoInt()
            {
                Minimum = new NumericBound<int>(0, true),
                Maximum = new NumericBound<int>(10, true)
            };
            var result = NumericValidation.Validate(info, 20);
            Assert.AreEqual(ValidationResultLevel.Error, result.Severity);
        }
    }
}
