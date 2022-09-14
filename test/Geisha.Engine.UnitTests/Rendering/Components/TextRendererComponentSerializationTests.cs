using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class TextRendererComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            const bool visible = false;
            const string sortingLayerName = "Some sorting layer";
            const int orderInLayer = 2;
            const string text = "some text";
            var fontSize = FontSize.FromPoints(12.34);
            var color = Color.FromArgb(1, 2, 3, 4);

            // Act
            var actual = SerializeAndDeserialize<TextRendererComponent>(component =>
            {
                component.Visible = visible;
                component.SortingLayerName = sortingLayerName;
                component.OrderInLayer = orderInLayer;
                component.Text = text;
                component.FontSize = fontSize;
                component.Color = color;
            });

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(sortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(orderInLayer));
            Assert.That(actual.Text, Is.EqualTo(text));
            Assert.That(actual.FontSize, Is.EqualTo(fontSize));
            Assert.That(actual.Color, Is.EqualTo(color));
        }
    }
}