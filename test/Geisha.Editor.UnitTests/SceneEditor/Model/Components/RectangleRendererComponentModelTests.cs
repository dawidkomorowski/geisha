using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model.Components
{
    [TestFixture]
    public class RectangleRendererComponentModelTests
    {
        private RectangleRendererComponent _rectangleRendererComponent;
        private RectangleRendererComponentModel _rectangleRendererComponentModel;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _rectangleRendererComponent = new RectangleRendererComponent
            {
                Dimension = new Vector2(1, 2),
                Color = Color.FromArgb(1, 2, 3, 4),
                FillInterior = true,
                Visible = true,
                SortingLayerName = "Test Layer",
                OrderInLayer = 1
            };
            _rectangleRendererComponentModel = new RectangleRendererComponentModel(_rectangleRendererComponent);
        }

        [Test]
        public void Dimension_ShouldUpdateRectangleRendererComponentDimension()
        {
            // Assume
            Assume.That(_rectangleRendererComponentModel.Dimension, Is.EqualTo(new Vector2(1, 2)));

            // Act
            _rectangleRendererComponentModel.Dimension = new Vector2(123, 456);

            // Assert
            Assert.That(_rectangleRendererComponentModel.Dimension, Is.EqualTo(new Vector2(123, 456)));
            Assert.That(_rectangleRendererComponent.Dimension, Is.EqualTo(new Vector2(123, 456)));
        }

        [Test]
        public void Color_ShouldUpdateRectangleRendererComponentColor()
        {
            // Arrange
            Assume.That(_rectangleRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(1, 2, 3, 4)));

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
            Assume.That(_rectangleRendererComponentModel.FillInterior, Is.True);

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
            Assume.That(_rectangleRendererComponentModel.Visible, Is.True);

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
            Assume.That(_rectangleRendererComponentModel.SortingLayerName, Is.EqualTo("Test Layer"));

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
            Assume.That(_rectangleRendererComponentModel.OrderInLayer, Is.EqualTo(1));

            // Act
            _rectangleRendererComponentModel.OrderInLayer = 123;

            // Assert
            Assert.That(_rectangleRendererComponentModel.OrderInLayer, Is.EqualTo(123));
            Assert.That(_rectangleRendererComponent.OrderInLayer, Is.EqualTo(123));
        }
    }
}