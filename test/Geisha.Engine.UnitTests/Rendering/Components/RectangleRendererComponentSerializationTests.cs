using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class RectangleRendererComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            const bool visible = false;
            const string sortingLayerName = "Some sorting layer";
            const int orderInLayer = 2;
            var dimension = new Vector2(1.23, 4.56);
            var color = Color.FromArgb(1, 2, 3, 4);
            const bool fillInterior = true;

            // Act
            var actual = SerializeAndDeserialize<RectangleRendererComponent>(component =>
            {
                component.Visible = visible;
                component.SortingLayerName = sortingLayerName;
                component.OrderInLayer = orderInLayer;
                component.Dimension = dimension;
                component.Color = color;
                component.FillInterior = fillInterior;
            });

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(sortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(orderInLayer));
            Assert.That(actual.Dimension, Is.EqualTo(dimension));
            Assert.That(actual.Color, Is.EqualTo(color));
            Assert.That(actual.FillInterior, Is.EqualTo(fillInterior));
        }
    }
}