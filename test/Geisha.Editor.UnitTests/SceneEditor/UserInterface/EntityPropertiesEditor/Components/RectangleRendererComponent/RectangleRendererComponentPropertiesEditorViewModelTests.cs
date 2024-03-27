using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleRendererComponent;
using Geisha.Engine.Core.Math;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleRendererComponent
{
    [TestFixture]
    public class RectangleRendererComponentPropertiesEditorViewModelTests
    {
        private RectangleRendererComponentModel _rectangleRendererComponentModel = null!;
        private RectangleRendererComponentPropertiesEditorViewModel _rectangleRendererComponentPropertiesEditorViewModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();

            var rectangleRendererComponent = entity.CreateComponent<Engine.Rendering.Components.RectangleRendererComponent>();
            rectangleRendererComponent.Dimensions = new Vector2(1, 2);
            rectangleRendererComponent.Color = Color.FromArgb(1, 2, 3, 4);
            rectangleRendererComponent.FillInterior = true;
            rectangleRendererComponent.Visible = true;
            rectangleRendererComponent.SortingLayerName = "Test Layer";
            rectangleRendererComponent.OrderInLayer = 1;

            _rectangleRendererComponentModel = new RectangleRendererComponentModel(rectangleRendererComponent);
            _rectangleRendererComponentPropertiesEditorViewModel = new RectangleRendererComponentPropertiesEditorViewModel(_rectangleRendererComponentModel);
        }

        [Test]
        public void Dimensions_ShouldUpdateRectangleRendererComponentModelDimensions()
        {
            // Assume
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.Dimensions, Is.EqualTo(new Vector2(1, 2)));

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.Dimensions = new Vector2(123, 456);

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.Dimensions, Is.EqualTo(new Vector2(123, 456)));
            Assert.That(_rectangleRendererComponentModel.Dimensions, Is.EqualTo(new Vector2(123, 456)));
        }

        [Test]
        public void Color_ShouldUpdateRectangleRendererComponentModelColor()
        {
            // Assume
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(1, 2, 3, 4)));

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.Color = Color.FromArgb(11, 22, 33, 44);

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
            Assert.That(_rectangleRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
        }

        [Test]
        public void FillInterior_ShouldUpdateRectangleRendererComponentModelFillInterior()
        {
            // Assume
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.FillInterior, Is.True);

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.FillInterior = false;

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.FillInterior, Is.False);
            Assert.That(_rectangleRendererComponentModel.FillInterior, Is.False);
        }

        [Test]
        public void Visible_ShouldUpdateRectangleRendererComponentModelVisible()
        {
            // Assume
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.Visible, Is.True);

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.Visible = false;

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.Visible, Is.False);
            Assert.That(_rectangleRendererComponentModel.Visible, Is.False);
        }

        [Test]
        public void SortingLayerName_ShouldUpdateRectangleRendererComponentSortingLayerName()
        {
            // Assume
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Test Layer"));

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.SortingLayerName = "Other Layer";

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Other Layer"));
            Assert.That(_rectangleRendererComponentModel.SortingLayerName, Is.EqualTo("Other Layer"));
        }

        [Test]
        public void OrderInLayer_ShouldUpdateRectangleRendererComponentOrderInLayer()
        {
            // Assume
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(1));

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.OrderInLayer = 123;

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(123));
            Assert.That(_rectangleRendererComponentModel.OrderInLayer, Is.EqualTo(123));
        }
    }
}