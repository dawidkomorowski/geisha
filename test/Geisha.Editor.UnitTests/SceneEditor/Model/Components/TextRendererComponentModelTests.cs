using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
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
            _textRendererComponent.FontSize = FontSize.FromDips(1);
            _textRendererComponent.Color = Color.FromArgb(1, 2, 3, 4);
            _textRendererComponent.Visible = true;
            _textRendererComponent.SortingLayerName = "Test Layer";
            _textRendererComponent.OrderInLayer = 1;

            _textRendererComponentModel = new TextRendererComponentModel(_textRendererComponent);
        }

        [Test]
        public void Text_ShouldUpdateTextRendererComponentText()
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
        public void FontSize_ShouldUpdateTextRendererComponentFontSize()
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
        public void Color_ShouldUpdateTextRendererComponentColor()
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
        public void Visible_ShouldUpdateTextRendererComponentVisible()
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
        public void SortingLayerName_ShouldUpdateTextRendererComponentSortingLayerName()
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
        public void OrderInLayer_ShouldUpdateTextRendererComponentOrderInLayer()
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