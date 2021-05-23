using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class EllipseRendererComponentSerializerTests : ComponentSerializerTestsBase
    {
        protected override IComponentFactory ComponentFactory => new EllipseRendererComponentFactory();

        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var component = new EllipseRendererComponent
            {
                Visible = false,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                RadiusX = 1.23,
                RadiusY = 4.56,
                Color = Color.FromArgb(1, 2, 3, 4),
                FillInterior = true
            };

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(component.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(component.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(component.OrderInLayer));
            Assert.That(actual.RadiusX, Is.EqualTo(component.RadiusX));
            Assert.That(actual.RadiusY, Is.EqualTo(component.RadiusY));
            Assert.That(actual.Color, Is.EqualTo(component.Color));
            Assert.That(actual.FillInterior, Is.EqualTo(component.FillInterior));
        }
    }
}