using Geisha.Common.Math;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Components.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components.Serialization
{
    [TestFixture]
    public class SerializableEllipseRendererComponentMapperTests
    {
        [Test]
        public void MapToSerializable()
        {
            // Arrange
            var mapper = new SerializableEllipseRendererComponentMapper();
            var ellipseRenderer = new EllipseRendererComponent
            {
                Visible = true,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                RadiusX = 1.23,
                RadiusY = 4.56,
                Color = Color.FromArgb(1, 2, 3, 4),
                FillInterior = true
            };

            // Act
            var actual = (SerializableEllipseRendererComponent) mapper.MapToSerializable(ellipseRenderer);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(ellipseRenderer.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(ellipseRenderer.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(ellipseRenderer.OrderInLayer));
            Assert.That(actual.RadiusX, Is.EqualTo(ellipseRenderer.RadiusX));
            Assert.That(actual.RadiusY, Is.EqualTo(ellipseRenderer.RadiusY));
            Assert.That(actual.ColorArgb, Is.EqualTo(ellipseRenderer.Color.ToArgb()));
            Assert.That(actual.FillInterior, Is.EqualTo(ellipseRenderer.FillInterior));
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableEllipseRendererComponentMapper();
            var serializableEllipseRenderer = new SerializableEllipseRendererComponent
            {
                Visible = true,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                RadiusX = 1.23,
                RadiusY = 4.56,
                ColorArgb = Color.FromArgb(1, 2, 3, 4).ToArgb(),
                FillInterior = true
            };

            // Act
            var actual = (EllipseRendererComponent) mapper.MapFromSerializable(serializableEllipseRenderer);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(serializableEllipseRenderer.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(serializableEllipseRenderer.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(serializableEllipseRenderer.OrderInLayer));
            Assert.That(actual.RadiusX, Is.EqualTo(serializableEllipseRenderer.RadiusX));
            Assert.That(actual.RadiusY, Is.EqualTo(serializableEllipseRenderer.RadiusY));
            Assert.That(actual.Color.ToArgb(), Is.EqualTo(serializableEllipseRenderer.ColorArgb));
            Assert.That(actual.FillInterior, Is.EqualTo(serializableEllipseRenderer.FillInterior));
        }
    }
}