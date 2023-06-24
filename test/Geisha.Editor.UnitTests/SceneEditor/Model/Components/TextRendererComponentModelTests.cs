using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model.Components
{
    [TestFixture]
    public class TextRendererComponentModelTests
    {
        private TextRendererComponent _textRendererComponent = null!;
        private TextRendererComponentModel _textRendererComponentModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();

            _textRendererComponent = entity.CreateComponent<TextRendererComponent>();
            _textRendererComponent.Text = "Some Text";
            _textRendererComponent.FontFamilyName = "Arial";
            _textRendererComponent.FontSize = FontSize.FromDips(1);
            _textRendererComponent.Color = Color.FromArgb(1, 2, 3, 4);
            _textRendererComponent.MaxWidth = 100;
            _textRendererComponent.MaxHeight = 200;
            _textRendererComponent.TextAlignment = TextAlignment.Leading;
            _textRendererComponent.ParagraphAlignment = ParagraphAlignment.Near;
            _textRendererComponent.Pivot = new Vector2(1, 2);
            _textRendererComponent.ClipToLayoutBox = false;
            _textRendererComponent.Visible = true;
            _textRendererComponent.SortingLayerName = "Test Layer";
            _textRendererComponent.OrderInLayer = 1;

            _textRendererComponentModel = new TextRendererComponentModel(_textRendererComponent);
        }

        [Test]
        public void Text_ShouldUpdateTextRendererComponent_Text()
        {
            // Assume
            Assume.That(_textRendererComponentModel.Text, Is.EqualTo("Some Text"));

            // Act
            _textRendererComponentModel.Text = "Other Text";

            // Assert
            Assert.That(_textRendererComponentModel.Text, Is.EqualTo("Other Text"));
            Assert.That(_textRendererComponent.Text, Is.EqualTo("Other Text"));
        }

        [Test]
        public void FontFamilyName_ShouldUpdateTextRendererComponent_FontFamilyName()
        {
            // Assume
            Assume.That(_textRendererComponentModel.FontFamilyName, Is.EqualTo("Arial"));

            // Act
            _textRendererComponentModel.FontFamilyName = "Calibri";

            // Assert
            Assert.That(_textRendererComponentModel.FontFamilyName, Is.EqualTo("Calibri"));
            Assert.That(_textRendererComponent.FontFamilyName, Is.EqualTo("Calibri"));
        }

        [Test]
        public void FontSize_ShouldUpdateTextRendererComponent_FontSize()
        {
            // Assume
            Assume.That(_textRendererComponentModel.FontSize, Is.EqualTo(FontSize.FromDips(1)));

            // Act
            _textRendererComponentModel.FontSize = FontSize.FromDips(123);

            // Assert
            Assert.That(_textRendererComponentModel.FontSize, Is.EqualTo(FontSize.FromDips(123)));
            Assert.That(_textRendererComponent.FontSize, Is.EqualTo(FontSize.FromDips(123)));
        }

        [Test]
        public void Color_ShouldUpdateTextRendererComponent_Color()
        {
            // Assume
            Assume.That(_textRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(1, 2, 3, 4)));

            // Act
            _textRendererComponentModel.Color = Color.FromArgb(11, 22, 33, 44);

            // Assert
            Assert.That(_textRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
            Assert.That(_textRendererComponent.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
        }

        [Test]
        public void MaxWidth_ShouldUpdateTextRendererComponent_MaxWidth()
        {
            // Assume
            Assume.That(_textRendererComponentModel.MaxWidth, Is.EqualTo(100));

            // Act
            _textRendererComponentModel.MaxWidth = 300;

            // Assert
            Assert.That(_textRendererComponentModel.MaxWidth, Is.EqualTo(300));
            Assert.That(_textRendererComponent.MaxWidth, Is.EqualTo(300));
        }

        [Test]
        public void MaxHeight_ShouldUpdateTextRendererComponent_MaxHeight()
        {
            // Assume
            Assume.That(_textRendererComponentModel.MaxHeight, Is.EqualTo(200));

            // Act
            _textRendererComponentModel.MaxHeight = 400;

            // Assert
            Assert.That(_textRendererComponentModel.MaxHeight, Is.EqualTo(400));
            Assert.That(_textRendererComponent.MaxHeight, Is.EqualTo(400));
        }

        [Test]
        public void TextAlignment_ShouldUpdateTextRendererComponent_TextAlignment()
        {
            // Assume
            Assume.That(_textRendererComponentModel.TextAlignment, Is.EqualTo(TextAlignment.Leading));

            // Act
            _textRendererComponentModel.TextAlignment = TextAlignment.Center;

            // Assert
            Assert.That(_textRendererComponentModel.TextAlignment, Is.EqualTo(TextAlignment.Center));
            Assert.That(_textRendererComponent.TextAlignment, Is.EqualTo(TextAlignment.Center));
        }

        [Test]
        public void ParagraphAlignment_ShouldUpdateTextRendererComponent_ParagraphAlignment()
        {
            // Assume
            Assume.That(_textRendererComponentModel.ParagraphAlignment, Is.EqualTo(ParagraphAlignment.Near));

            // Act
            _textRendererComponentModel.ParagraphAlignment = ParagraphAlignment.Center;

            // Assert
            Assert.That(_textRendererComponentModel.ParagraphAlignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(_textRendererComponent.ParagraphAlignment, Is.EqualTo(ParagraphAlignment.Center));
        }

        [Test]
        public void Pivot_ShouldUpdateTextRendererComponent_Pivot()
        {
            // Assume
            Assume.That(_textRendererComponentModel.Pivot, Is.EqualTo(new Vector2(1, 2)));

            // Act
            _textRendererComponentModel.Pivot = new Vector2(3, 4);

            // Assert
            Assert.That(_textRendererComponentModel.Pivot, Is.EqualTo(new Vector2(3, 4)));
            Assert.That(_textRendererComponent.Pivot, Is.EqualTo(new Vector2(3, 4)));
        }

        [Test]
        public void ClipToLayoutBox_ShouldUpdateTextRendererComponent_ClipToLayoutBox()
        {
            // Assume
            Assume.That(_textRendererComponentModel.ClipToLayoutBox, Is.False);

            // Act
            _textRendererComponentModel.ClipToLayoutBox = true;

            // Assert
            Assert.That(_textRendererComponentModel.ClipToLayoutBox, Is.True);
            Assert.That(_textRendererComponent.ClipToLayoutBox, Is.True);
        }

        [Test]
        public void Visible_ShouldUpdateTextRendererComponent_Visible()
        {
            // Assume
            Assume.That(_textRendererComponentModel.Visible, Is.True);

            // Act
            _textRendererComponentModel.Visible = false;

            // Assert
            Assert.That(_textRendererComponentModel.Visible, Is.False);
            Assert.That(_textRendererComponent.Visible, Is.False);
        }

        [Test]
        public void SortingLayerName_ShouldUpdateTextRendererComponent_SortingLayerName()
        {
            // Assume
            Assume.That(_textRendererComponentModel.SortingLayerName, Is.EqualTo("Test Layer"));

            // Act
            _textRendererComponentModel.SortingLayerName = "Other Layer";

            // Assert
            Assert.That(_textRendererComponentModel.SortingLayerName, Is.EqualTo("Other Layer"));
            Assert.That(_textRendererComponent.SortingLayerName, Is.EqualTo("Other Layer"));
        }

        [Test]
        public void OrderInLayer_ShouldUpdateTextRendererComponent_OrderInLayer()
        {
            // Assume
            Assume.That(_textRendererComponentModel.OrderInLayer, Is.EqualTo(1));

            // Act
            _textRendererComponentModel.OrderInLayer = 123;

            // Assert
            Assert.That(_textRendererComponentModel.OrderInLayer, Is.EqualTo(123));
            Assert.That(_textRendererComponent.OrderInLayer, Is.EqualTo(123));
        }
    }
}