using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
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
            const string fontFamilyName = "Arial";
            var fontSize = FontSize.FromPoints(12.34);
            var color = Color.FromArgb(1, 2, 3, 4);
            const double maxWidth = 1000.123;
            const double maxHeight = 2000.123;
            const TextAlignment textAlignment = TextAlignment.Center;
            const ParagraphAlignment paragraphAlignment = ParagraphAlignment.Center;
            var pivot = new Vector2(200, 300);
            const bool clipToLayoutBox = true;

            // Act
            var actual = SerializeAndDeserialize<TextRendererComponent>(component =>
            {
                component.Visible = visible;
                component.SortingLayerName = sortingLayerName;
                component.OrderInLayer = orderInLayer;
                component.Text = text;
                component.FontFamilyName = fontFamilyName;
                component.FontSize = fontSize;
                component.Color = color;
                component.MaxWidth = maxWidth;
                component.MaxHeight = maxHeight;
                component.TextAlignment = textAlignment;
                component.ParagraphAlignment = paragraphAlignment;
                component.Pivot = pivot;
                component.ClipToLayoutBox = clipToLayoutBox;
            });

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(sortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(orderInLayer));
            Assert.That(actual.Text, Is.EqualTo(text));
            Assert.That(actual.FontFamilyName, Is.EqualTo(fontFamilyName));
            Assert.That(actual.FontSize, Is.EqualTo(fontSize));
            Assert.That(actual.Color, Is.EqualTo(color));
            Assert.That(actual.MaxWidth, Is.EqualTo(maxWidth));
            Assert.That(actual.MaxHeight, Is.EqualTo(maxHeight));
            Assert.That(actual.TextAlignment, Is.EqualTo(textAlignment));
            Assert.That(actual.ParagraphAlignment, Is.EqualTo(paragraphAlignment));
            Assert.That(actual.Pivot, Is.EqualTo(pivot));
            Assert.That(actual.ClipToLayoutBox, Is.EqualTo(clipToLayoutBox));
        }
    }
}