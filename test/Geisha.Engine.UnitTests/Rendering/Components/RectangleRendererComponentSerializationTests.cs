using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class RectangleRendererComponentSerializationTests : ComponentSerializationTestsBase
    {
        protected override IComponentFactory ComponentFactory => new RectangleRendererComponentFactory();

        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var component = new RectangleRendererComponent
            {
                Visible = false,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Dimension = new Vector2(1.23, 4.56),
                Color = Color.FromArgb(1, 2, 3, 4),
                FillInterior = true
            };

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(component.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(component.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(component.OrderInLayer));
            Assert.That(actual.Dimension, Is.EqualTo(component.Dimension));
            Assert.That(actual.Color, Is.EqualTo(component.Color));
            Assert.That(actual.FillInterior, Is.EqualTo(component.FillInterior));
        }
    }
}