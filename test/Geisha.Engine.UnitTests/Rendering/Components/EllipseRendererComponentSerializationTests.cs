using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class EllipseRendererComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            const bool visible = false;
            const string sortingLayerName = "Some sorting layer";
            const int orderInLayer = 2;
            const double radiusX = 1.23;
            const double radiusY = 4.56;
            var color = Color.FromArgb(1, 2, 3, 4);
            const bool fillInterior = true;

            // Act
            var actual = SerializeAndDeserialize<EllipseRendererComponent>(component =>
            {
                component.Visible = visible;
                component.SortingLayerName = sortingLayerName;
                component.OrderInLayer = orderInLayer;
                component.RadiusX = radiusX;
                component.RadiusY = radiusY;
                component.Color = color;
                component.FillInterior = fillInterior;
            });

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(sortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(orderInLayer));
            Assert.That(actual.RadiusX, Is.EqualTo(radiusX));
            Assert.That(actual.RadiusY, Is.EqualTo(radiusY));
            Assert.That(actual.Color, Is.EqualTo(color));
            Assert.That(actual.FillInterior, Is.EqualTo(fillInterior));
        }
    }
}