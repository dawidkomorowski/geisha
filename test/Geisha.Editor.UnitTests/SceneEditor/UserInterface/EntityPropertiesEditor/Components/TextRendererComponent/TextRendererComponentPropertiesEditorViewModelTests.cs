using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TextRendererComponent;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TextRendererComponent
{
    [TestFixture]
    public class TextRendererComponentPropertiesEditorViewModelTests
    {
        private TextRendererComponentModel _textRendererComponentModel = null!;
        private TextRendererComponentPropertiesEditorViewModel _textRendererComponentPropertiesEditorViewModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();

            var textRendererComponent = entity.CreateComponent<Engine.Rendering.Components.TextRendererComponent>();
            textRendererComponent.Text = "Some Text";
            textRendererComponent.FontFamilyName = "Arial";
            textRendererComponent.FontSize = FontSize.FromDips(1);
            textRendererComponent.Color = Color.FromArgb(1, 2, 3, 4);
            textRendererComponent.MaxWidth = 100;
            textRendererComponent.MaxHeight = 200;
            textRendererComponent.TextAlignment = TextAlignment.Leading;
            textRendererComponent.ParagraphAlignment = ParagraphAlignment.Near;
            textRendererComponent.Pivot = new Vector2(1, 2);
            textRendererComponent.ClipToLayoutBox = false;
            textRendererComponent.Visible = true;
            textRendererComponent.SortingLayerName = "Test Layer";
            textRendererComponent.OrderInLayer = 1;

            _textRendererComponentModel = new TextRendererComponentModel(textRendererComponent);
            _textRendererComponentPropertiesEditorViewModel = new TextRendererComponentPropertiesEditorViewModel(_textRendererComponentModel);
        }

        [Test]
        public void Text_ShouldUpdateTextRendererComponentModel_Text()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Text, Is.EqualTo("Some Text"));

            // Act
            _textRendererComponentPropertiesEditorViewModel.Text = "Other Text";

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Text, Is.EqualTo("Other Text"));
            Assert.That(_textRendererComponentModel.Text, Is.EqualTo("Other Text"));
        }

        [Test]
        public void FontFamilyName_ShouldUpdateTextRendererComponentModel_FontFamilyName()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.FontFamilyName, Is.EqualTo("Arial"));

            // Act
            _textRendererComponentPropertiesEditorViewModel.FontFamilyName = "Calibri";

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.FontFamilyName, Is.EqualTo("Calibri"));
            Assert.That(_textRendererComponentModel.FontFamilyName, Is.EqualTo("Calibri"));
        }

        [Test]
        public void FontSize_ShouldUpdateTextRendererComponentModel_FontSize()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.FontSize, Is.EqualTo(FontSize.FromDips(1)));

            // Act
            _textRendererComponentPropertiesEditorViewModel.FontSize = FontSize.FromDips(123);

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.FontSize, Is.EqualTo(FontSize.FromDips(123)));
            Assert.That(_textRendererComponentModel.FontSize, Is.EqualTo(FontSize.FromDips(123)));
        }

        [Test]
        public void Color_ShouldUpdateTextRendererComponentModel_Color()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(1, 2, 3, 4)));

            // Act
            _textRendererComponentPropertiesEditorViewModel.Color = Color.FromArgb(11, 22, 33, 44);

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
            Assert.That(_textRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
        }

        [Test]
        public void MaxWidth_ShouldUpdateTextRendererComponentModel_MaxWidth()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.MaxWidth, Is.EqualTo(100));

            // Act
            _textRendererComponentPropertiesEditorViewModel.MaxWidth = 300;

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.MaxWidth, Is.EqualTo(300));
            Assert.That(_textRendererComponentModel.MaxWidth, Is.EqualTo(300));
        }

        [Test]
        public void MaxHeight_ShouldUpdateTextRendererComponentModel_MaxHeight()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.MaxHeight, Is.EqualTo(200));

            // Act
            _textRendererComponentPropertiesEditorViewModel.MaxHeight = 400;

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.MaxHeight, Is.EqualTo(400));
            Assert.That(_textRendererComponentModel.MaxHeight, Is.EqualTo(400));
        }

        [Test]
        public void TextAlignment_ShouldUpdateTextRendererComponentModel_TextAlignment()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.TextAlignment, Is.EqualTo(TextAlignment.Leading));

            // Act
            _textRendererComponentPropertiesEditorViewModel.TextAlignment = TextAlignment.Center;

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.TextAlignment, Is.EqualTo(TextAlignment.Center));
            Assert.That(_textRendererComponentModel.TextAlignment, Is.EqualTo(TextAlignment.Center));
        }

        [Test]
        public void ParagraphAlignment_ShouldUpdateTextRendererComponentModel_ParagraphAlignment()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.ParagraphAlignment, Is.EqualTo(ParagraphAlignment.Near));

            // Act
            _textRendererComponentPropertiesEditorViewModel.ParagraphAlignment = ParagraphAlignment.Center;

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.ParagraphAlignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(_textRendererComponentModel.ParagraphAlignment, Is.EqualTo(ParagraphAlignment.Center));
        }

        [Test]
        public void Pivot_ShouldUpdateTextRendererComponentModel_Pivot()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Pivot, Is.EqualTo(new Vector2(1, 2)));

            // Act
            _textRendererComponentPropertiesEditorViewModel.Pivot = new Vector2(1, 2);

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Pivot, Is.EqualTo(new Vector2(1, 2)));
            Assert.That(_textRendererComponentModel.Pivot, Is.EqualTo(new Vector2(1, 2)));
        }

        [Test]
        public void ClipToLayoutBox_ShouldUpdateTextRendererComponentModel_ClipToLayoutBox()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.ClipToLayoutBox, Is.False);

            // Act
            _textRendererComponentPropertiesEditorViewModel.ClipToLayoutBox = true;

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.ClipToLayoutBox, Is.True);
            Assert.That(_textRendererComponentModel.ClipToLayoutBox, Is.True);
        }

        [Test]
        public void Visible_ShouldUpdateTextRendererComponentModel_Visible()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Visible, Is.True);

            // Act
            _textRendererComponentPropertiesEditorViewModel.Visible = false;

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Visible, Is.False);
            Assert.That(_textRendererComponentModel.Visible, Is.False);
        }

        [Test]
        public void SortingLayerName_ShouldUpdateTextRendererComponentModel_SortingLayerName()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Test Layer"));

            // Act
            _textRendererComponentPropertiesEditorViewModel.SortingLayerName = "Other Layer";

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Other Layer"));
            Assert.That(_textRendererComponentModel.SortingLayerName, Is.EqualTo("Other Layer"));
        }

        [Test]
        public void OrderInLayer_ShouldUpdateTextRendererComponentModel_OrderInLayer()
        {
            // Assume
            Assert.That(_textRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(1));

            // Act
            _textRendererComponentPropertiesEditorViewModel.OrderInLayer = 123;

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(123));
            Assert.That(_textRendererComponentModel.OrderInLayer, Is.EqualTo(123));
        }
    }
}