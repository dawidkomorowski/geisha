using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model.Components
{
    [TestFixture]
    public class RectangleRendererComponentModelTests
    {
        private RectangleRendererComponent _rectangleRendererComponent = null!;
        private RectangleRendererComponentModel _rectangleRendererComponentModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();

            _rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();
            _rectangleRendererComponent.Dimensions = new Vector2(1, 2);
            _rectangleRendererComponent.Color = Color.FromArgb(1, 2, 3, 4);
            _rectangleRendererComponent.FillInterior = true;
            _rectangleRendererComponent.Visible = true;
            _rectangleRendererComponent.SortingLayerName = "Test Layer";
            _rectangleRendererComponent.OrderInLayer = 1;

            _rectangleRendererComponentModel = new RectangleRendererComponentModel(_rectangleRendererComponent);
        }

        [Test]
        public void Dimensions_ShouldUpdateRectangleRendererComponentDimensions()
        {
            // Assume
            Assert.That(_rectangleRendererComponentModel.Dimensions, Is.EqualTo(new Vector2(1, 2)));

            // Act
            _rectangleRendererComponentModel.Dimensions = new Vector2(123, 456);

            // Assert
            Assert.That(_rectangleRendererComponentModel.Dimensions, Is.EqualTo(new Vector2(123, 456)));
            Assert.That(_rectangleRendererComponent.Dimensions, Is.EqualTo(new Vector2(123, 456)));
        }

        [Test]
        public void Color_ShouldUpdateRectangleRendererComponentColor()
        {
            // Arrange
            Assert.That(_rectangleRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(1, 2, 3, 4)));

            // Act
            _rectangleRendererComponentModel.Color = Color.FromArgb(11, 22, 33, 44);

            // Assert
            Assert.That(_rectangleRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
            Assert.That(_rectangleRendererComponent.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
        }

        [Test]
        public void FillInterior_ShouldUpdateRectangleRendererComponentFillInterior()
        {
            // Assume
            Assert.That(_rectangleRendererComponentModel.FillInterior, Is.True);

            // Act
            _rectangleRendererComponentModel.FillInterior = false;

            // Assert
            Assert.That(_rectangleRendererComponentModel.FillInterior, Is.False);
            Assert.That(_rectangleRendererComponent.FillInterior, Is.False);
        }

        [Test]
        public void Visible_ShouldUpdateRectangleRendererComponentVisible()
        {
            // Assume
            Assert.That(_rectangleRendererComponentModel.Visible, Is.True);

            // Act
            _rectangleRendererComponentModel.Visible = false;

            // Assert
            Assert.That(_rectangleRendererComponentModel.Visible, Is.False);
            Assert.That(_rectangleRendererComponent.Visible, Is.False);
        }

        [Test]
        public void SortingLayerName_ShouldUpdateRectangleRendererComponentSortingLayerName()
        {
            // Assume
            Assert.That(_rectangleRendererComponentModel.SortingLayerName, Is.EqualTo("Test Layer"));

            // Act
            _rectangleRendererComponentModel.SortingLayerName = "Other Layer";

            // Assert
            Assert.That(_rectangleRendererComponentModel.SortingLayerName, Is.EqualTo("Other Layer"));
            Assert.That(_rectangleRendererComponent.SortingLayerName, Is.EqualTo("Other Layer"));
        }

        [Test]
        public void OrderInLayer_ShouldUpdateRectangleRendererComponentOrderInLayer()
        {
            // Assume
            Assert.That(_rectangleRendererComponentModel.OrderInLayer, Is.EqualTo(1));

            // Act
            _rectangleRendererComponentModel.OrderInLayer = 123;

            // Assert
            Assert.That(_rectangleRendererComponentModel.OrderInLayer, Is.EqualTo(123));
            Assert.That(_rectangleRendererComponent.OrderInLayer, Is.EqualTo(123));
        }
    }
}