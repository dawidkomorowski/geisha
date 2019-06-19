using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Components.Serialization;
using Geisha.Framework.Rendering;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components.Serialization
{
    [TestFixture]
    public class SerializableRectangleRendererComponentMapperTests
    {
        [Test]
        public void MapToSerializable()
        {
            // Arrange
            var mapper = new SerializableRectangleRendererComponentMapper();
            var rectangleRenderer = new RectangleRendererComponent
            {
                Visible = true,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Dimension = new Vector2(1.23, -3.21),
                Color = Color.FromArgb(1, 2, 3, 4),
                FillInterior = true
            };

            // Act
            var actual = (SerializableRectangleRendererComponent) mapper.MapToSerializable(rectangleRenderer);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(rectangleRenderer.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(rectangleRenderer.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(rectangleRenderer.OrderInLayer));
            Assert.That(actual.Dimension.X, Is.EqualTo(rectangleRenderer.Dimension.X));
            Assert.That(actual.Dimension.Y, Is.EqualTo(rectangleRenderer.Dimension.Y));
            Assert.That(actual.ColorArgb, Is.EqualTo(rectangleRenderer.Color.ToArgb()));
            Assert.That(actual.FillInterior, Is.EqualTo(rectangleRenderer.FillInterior));
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableRectangleRendererComponentMapper();
            var serializableRectangleRenderer = new SerializableRectangleRendererComponent
            {
                Visible = true,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Dimension = new SerializableVector2 {X = 1.23, Y = -3.21},
                ColorArgb = Color.FromArgb(1, 2, 3, 4).ToArgb(),
                FillInterior = true
            };

            // Act
            var actual = (RectangleRendererComponent) mapper.MapFromSerializable(serializableRectangleRenderer);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(serializableRectangleRenderer.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(serializableRectangleRenderer.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(serializableRectangleRenderer.OrderInLayer));
            Assert.That(actual.Dimension.X, Is.EqualTo(serializableRectangleRenderer.Dimension.X));
            Assert.That(actual.Dimension.Y, Is.EqualTo(serializableRectangleRenderer.Dimension.Y));
            Assert.That(actual.Color.ToArgb(), Is.EqualTo(serializableRectangleRenderer.ColorArgb));
            Assert.That(actual.FillInterior, Is.EqualTo(serializableRectangleRenderer.FillInterior));
        }
    }
}