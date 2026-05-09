using System.Collections.Generic;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Mapping;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Mapping
{
    [TestFixture]
    public class HardwareInputVariantTests
    {
        #region Keyboard

        [Test]
        public void CreateKeyboardVariant_ShouldSetCurrentVariantToKeyboard()
        {
            // Arrange

            // Act
            var hardwareInputVariant = HardwareInputVariant.Create(Key.Space);

            // Assert
            Assert.That(hardwareInputVariant.CurrentVariant, Is.EqualTo(HardwareInputVariant.Variant.Keyboard));
        }

        [Test]
        public void AsKeyboard_ShouldReturnKeyboardKey_WhenVariantIsKeyboard()
        {
            // Arrange
            const Key key = Key.Space;
            var hardwareInputVariant = HardwareInputVariant.Create(key);

            // Act
            // Assert
            Assert.That(hardwareInputVariant.AsKeyboard(), Is.EqualTo(key));
        }

        [Test]
        public void AsKeyboard_ShouldThrowException_WhenVariantIsNotKeyboard()
        {
            // Arrange
            var hardwareInputVariant = HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.LeftButton);

            // Act
            // Assert
            Assert.That(hardwareInputVariant.AsKeyboard, Throws.InvalidOperationException);
        }

        #endregion

        #region Mouse

        [Test]
        public void CreateMouseVariant_ShouldSetCurrentVariantToMouse()
        {
            // Arrange

            // Act
            var hardwareInputVariant = HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.LeftButton);

            // Assert
            Assert.That(hardwareInputVariant.CurrentVariant, Is.EqualTo(HardwareInputVariant.Variant.Mouse));
        }

        [Test]
        public void AsMouse_ShouldReturnMouseVariant_WhenVariantIsMouse()
        {
            // Arrange
            const HardwareInputVariant.MouseVariant mouseVariant = HardwareInputVariant.MouseVariant.LeftButton;
            var hardwareInputVariant = HardwareInputVariant.Create(mouseVariant);

            // Act
            // Assert
            Assert.That(hardwareInputVariant.AsMouse(), Is.EqualTo(mouseVariant));
        }

        [Test]
        public void AsMouse_ShouldThrowException_WhenVariantIsNotMouse()
        {
            // Arrange
            var hardwareInputVariant = HardwareInputVariant.Create(Key.Space);

            // Act
            // Assert
            Assert.That(hardwareInputVariant.AsMouse, Throws.InvalidOperationException);
        }

        #endregion

        #region Formatting members

        private static IEnumerable<ToStringTestCase> ToStringTestCases => new[]
        {
            new ToStringTestCase(HardwareInputVariant.Create(Key.Space),
                "CurrentVariant: Keyboard, KeyboardVariant: Space"),
            new ToStringTestCase(HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.LeftButton),
                "CurrentVariant: Mouse, MouseVariant: LeftButton")
        };

        [TestCaseSource(nameof(ToStringTestCases))]
        public void ToString_Test(ToStringTestCase testCase)
        {
            // Arrange
            // Act
            var actual = testCase.HardwareInputVariant.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(testCase.Expected));
        }

        public sealed class ToStringTestCase
        {
            public ToStringTestCase(HardwareInputVariant hardwareInputVariant, string expected)
            {
                HardwareInputVariant = hardwareInputVariant;
                Expected = expected;
            }

            public HardwareInputVariant HardwareInputVariant { get; }
            public string Expected { get; }

            public override string ToString()
            {
                return Expected;
            }
        }

        #endregion

        #region Equality members

        private static IEnumerable<EqualityMembersTestCase> EqualityMembersTestCases => new[]
        {
            new EqualityMembersTestCase(HardwareInputVariant.Create(Key.Space),
                HardwareInputVariant.Create(Key.Space), true),
            new EqualityMembersTestCase(HardwareInputVariant.Create(Key.Space),
                HardwareInputVariant.Create(Key.Enter), false),
            new EqualityMembersTestCase(HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.LeftButton),
                HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.LeftButton), true),
            new EqualityMembersTestCase(HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.LeftButton),
                HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.RightButton), false),
            new EqualityMembersTestCase(HardwareInputVariant.Create(Key.Space),
                HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.RightButton), false)
        };

        [TestCaseSource(nameof(EqualityMembersTestCases))]
        public void EqualityMembers_Test(EqualityMembersTestCase testCase)
        {
            // Arrange
            var variant1 = testCase.HardwareInputVariant1;
            var variant2 = testCase.HardwareInputVariant2;

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(variant1, variant2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(testCase.ExpectedIsEqual);
        }

        public sealed class EqualityMembersTestCase
        {
            public EqualityMembersTestCase(HardwareInputVariant hardwareInputVariant1, HardwareInputVariant hardwareInputVariant2, bool expectedIsEqual)
            {
                HardwareInputVariant1 = hardwareInputVariant1;
                HardwareInputVariant2 = hardwareInputVariant2;
                ExpectedIsEqual = expectedIsEqual;
            }

            public HardwareInputVariant HardwareInputVariant1 { get; }
            public HardwareInputVariant HardwareInputVariant2 { get; }
            public bool ExpectedIsEqual { get; }

            public override string ToString()
            {
                var not = ExpectedIsEqual ? string.Empty : "not";
                return $"{HardwareInputVariant1} is {not} equal to {HardwareInputVariant2}.";
            }
        }

        #endregion
    }
}