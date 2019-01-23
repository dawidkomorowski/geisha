using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Components.Definition;
using Geisha.Framework.Rendering;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Components.Definition
{
    [TestFixture]
    public class TextRendererDefinitionMapperTests
    {
        [Test]
        public void ToDefinition()
        {
            // Arrange
            var mapper = new TextRendererDefinitionMapper();
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
            var actual = (TextRendererDefinition) mapper.MapToSerializable(textRenderer);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(textRenderer.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(textRenderer.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(textRenderer.OrderInLayer));
            Assert.That(actual.Text, Is.EqualTo(textRenderer.Text));
            Assert.That(actual.FontSize, Is.EqualTo(textRenderer.FontSize.Points));
            Assert.That(actual.ColorArgb, Is.EqualTo(textRenderer.Color.ToArgb()));
        }

        [Test]
        public void FromDefinition()
        {
            // Arrange
            var mapper = new TextRendererDefinitionMapper();
            var textRendererDefinition = new TextRendererDefinition
            {
                Visible = false,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Text = "some text",
                FontSize = 12,
                ColorArgb = Color.FromArgb(1, 2, 3, 4).ToArgb()
            };

            // Act
            var actual = (TextRendererComponent) mapper.MapFromSerializable(textRendererDefinition);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(textRendererDefinition.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(textRendererDefinition.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(textRendererDefinition.OrderInLayer));
            Assert.That(actual.Text, Is.EqualTo(textRendererDefinition.Text));
            Assert.That(actual.FontSize.Points, Is.EqualTo(textRendererDefinition.FontSize));
            Assert.That(actual.Color.ToArgb(), Is.EqualTo(textRendererDefinition.ColorArgb));
        }
    }
}