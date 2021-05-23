using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class TextRendererComponentSerializationTests : ComponentSerializationTestsBase
    {
        protected override IComponentFactory ComponentFactory => new TextRendererComponentFactory();

        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var component = new TextRendererComponent
            {
                Visible = false,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Text = "some text",
                FontSize = FontSize.FromPoints(12.34),
                Color = Color.FromArgb(1, 2, 3, 4)
            };

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(component.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(component.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(component.OrderInLayer));
            Assert.That(actual.Text, Is.EqualTo(component.Text));
            Assert.That(actual.FontSize, Is.EqualTo(component.FontSize));
            Assert.That(actual.Color, Is.EqualTo(component.Color));
        }
    }
}