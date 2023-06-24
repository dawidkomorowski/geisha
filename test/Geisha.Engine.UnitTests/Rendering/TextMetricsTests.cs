using Geisha.Engine.Rendering;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering
{
    [TestFixture]
    public class TextMetricsTests
    {
        [TestCase(10, 20, 100, 200, 400, 800, 15,
            10, 20, 100, 200, 400, 800, 15, true)]
        [TestCase(10, 20, 100, 200, 400, 800, 15,
            11, 20, 100, 200, 400, 800, 15, false)]
        [TestCase(10, 20, 100, 200, 400, 800, 15,
            10, 21, 100, 200, 400, 800, 15, false)]
        [TestCase(10, 20, 100, 200, 400, 800, 15,
            10, 20, 101, 200, 400, 800, 15, false)]
        [TestCase(10, 20, 100, 200, 400, 800, 15,
            10, 20, 100, 201, 400, 800, 15, false)]
        [TestCase(10, 20, 100, 200, 400, 800, 15,
            10, 20, 100, 200, 401, 800, 15, false)]
        [TestCase(10, 20, 100, 200, 400, 800, 15,
            10, 20, 100, 200, 400, 800, 115, false)]
        [TestCase(10, 20, 100, 200, 400, 800, 15,
            10, 20, 100, 200, 400, 800, 16, false)]
        public void EqualityMembers_ShouldEqualTextMetrics_WhenAllPropertiesAreEqual(
            double left1, double top1, double width1, double height1, double layoutWidth1, double layoutHeight1, int lineCount1,
            double left2, double top2, double width2, double height2, double layoutWidth2, double layoutHeight2, int lineCount2,
            bool expectedIsEqual)
        {
            // Arrange
            var textMetrics1 = new TextMetrics
            {
                Left = left1,
                Top = top1,
                Width = width1,
                Height = height1,
                LayoutWidth = layoutWidth1,
                LayoutHeight = layoutHeight1,
                LineCount = lineCount1
            };
            var textMetrics2 = new TextMetrics
            {
                Left = left2,
                Top = top2,
                Width = width2,
                Height = height2,
                LayoutWidth = layoutWidth2,
                LayoutHeight = layoutHeight2,
                LineCount = lineCount2
            };

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(textMetrics1, textMetrics2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }

        [Test]
        [SetCulture("")]
        public void ToString_Test()
        {
            // Arrange
            var textMetrics = new TextMetrics
            {
                Left = 10.1,
                Top = 20.2,
                Width = 100.3,
                Height = 200.4,
                LayoutWidth = 400.5,
                LayoutHeight = 800.6,
                LineCount = 15
            };

            // Act
            var actual = textMetrics.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo("Left: 10.1, Top: 20.2, Width: 100.3, Height: 200.4, LayoutWidth: 400.5, LayoutHeight: 800.6, LineCount: 15"));
        }
    }
}