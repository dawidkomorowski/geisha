using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TextRendererComponent;
using Geisha.Engine.Rendering;
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
            var textRendererComponent = new Engine.Rendering.Components.TextRendererComponent
            {
                Text = "Some Text",
                FontSize = FontSize.FromDips(1),
                Color = Color.FromArgb(1, 2, 3, 4),
                Visible = true,
                SortingLayerName = "Test Layer",
                OrderInLayer = 1
            };
            _textRendererComponentModel = new TextRendererComponentModel(textRendererComponent);
            _textRendererComponentPropertiesEditorViewModel = new TextRendererComponentPropertiesEditorViewModel(_textRendererComponentModel);
        }

        [Test]
        public void Text_ShouldUpdateTextRendererComponentModelText()
        {
            // Assume
            Assume.That(_textRendererComponentPropertiesEditorViewModel.Text, Is.EqualTo("Some Text"));

            // Act
            _textRendererComponentPropertiesEditorViewModel.Text = "Other Text";

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Text, Is.EqualTo("Other Text"));
            Assert.That(_textRendererComponentModel.Text, Is.EqualTo("Other Text"));
        }

        [Test]
        public void FontSize_ShouldUpdateTextRendererComponentModelFontSize()
        {
            // Assume
            Assume.That(_textRendererComponentPropertiesEditorViewModel.FontSize, Is.EqualTo(FontSize.FromDips(1)));

            // Act
            _textRendererComponentPropertiesEditorViewModel.FontSize = FontSize.FromDips(123);

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.FontSize, Is.EqualTo(FontSize.FromDips(123)));
            Assert.That(_textRendererComponentModel.FontSize, Is.EqualTo(FontSize.FromDips(123)));
        }

        [Test]
        public void Color_ShouldUpdateTextRendererComponentModelColor()
        {
            // Assume
            Assume.That(_textRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(1, 2, 3, 4)));

            // Act
            _textRendererComponentPropertiesEditorViewModel.Color = Color.FromArgb(11, 22, 33, 44);

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
            Assert.That(_textRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
        }

        [Test]
        public void Visible_ShouldUpdateTextRendererComponentModelVisible()
        {
            // Assume
            Assume.That(_textRendererComponentPropertiesEditorViewModel.Visible, Is.True);

            // Act
            _textRendererComponentPropertiesEditorViewModel.Visible = false;

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.Visible, Is.False);
            Assert.That(_textRendererComponentModel.Visible, Is.False);
        }

        [Test]
        public void SortingLayerName_ShouldUpdateTextRendererComponentModelSortingLayerName()
        {
            // Assume
            Assume.That(_textRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Test Layer"));

            // Act
            _textRendererComponentPropertiesEditorViewModel.SortingLayerName = "Other Layer";

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Other Layer"));
            Assert.That(_textRendererComponentModel.SortingLayerName, Is.EqualTo("Other Layer"));
        }

        [Test]
        public void OrderInLayer_ShouldUpdateTextRendererComponentModelOrderInLayer()
        {
            // Assume
            Assume.That(_textRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(1));

            // Act
            _textRendererComponentPropertiesEditorViewModel.OrderInLayer = 123;

            // Assert
            Assert.That(_textRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(123));
            Assert.That(_textRendererComponentModel.OrderInLayer, Is.EqualTo(123));
        }
    }
}