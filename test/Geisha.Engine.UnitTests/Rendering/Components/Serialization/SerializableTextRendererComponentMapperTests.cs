using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Components.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components.Serialization
{
    [TestFixture]
    public class SerializableTextRendererComponentMapperTests
    {
        [Test]
        public void MapToSerializable()
        {
            // Arrange
            var mapper = new SerializableTextRendererComponentMapper();
            var textRenderer = new TextRendererComponent
            {
                Visible = true,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Text = "some text",
                FontSize = FontSize.FromPoints(12.34),
                Color = Color.FromArgb(1, 2, 3, 4)
            };

            // Act
            var actual = (SerializableTextRendererComponent) mapper.MapToSerializable(textRenderer);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(textRenderer.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(textRenderer.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(textRenderer.OrderInLayer));
            Assert.That(actual.Text, Is.EqualTo(textRenderer.Text));
            Assert.That(actual.FontSize, Is.EqualTo(textRenderer.FontSize.Points));
            Assert.That(actual.ColorArgb, Is.EqualTo(textRenderer.Color.ToArgb()));
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var mapper = new SerializableTextRendererComponentMapper();
            var serializableTextRenderer = new SerializableTextRendererComponent
            {
                Visible = false,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Text = "some text",
                FontSize = 12,
                ColorArgb = Color.FromArgb(1, 2, 3, 4).ToArgb()
            };

            // Act
            var actual = (TextRendererComponent) mapper.MapFromSerializable(serializableTextRenderer);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(serializableTextRenderer.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(serializableTextRenderer.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(serializableTextRenderer.OrderInLayer));
            Assert.That(actual.Text, Is.EqualTo(serializableTextRenderer.Text));
            Assert.That(actual.FontSize.Points, Is.EqualTo(serializableTextRenderer.FontSize));
            Assert.That(actual.Color.ToArgb(), Is.EqualTo(serializableTextRenderer.ColorArgb));
        }
    }
}