using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Rendering.Components;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model.Components
{
    [TestFixture]
    public class EllipseRendererComponentModelTests
    {
        private EllipseRendererComponent _ellipseRendererComponent = null!;
        private EllipseRendererComponentModel _ellipseRendererComponentModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _ellipseRendererComponent = new EllipseRendererComponent
            {
                RadiusX = 1,
                RadiusY = 2,
                Color = Color.FromArgb(1, 2, 3, 4),
                FillInterior = true,
                Visible = true,
                SortingLayerName = "Test Layer",
                OrderInLayer = 1
            };
            _ellipseRendererComponentModel = new EllipseRendererComponentModel(_ellipseRendererComponent);
        }

        [Test]
        public void RadiusX_ShouldUpdateEllipseRendererComponentRadiusX()
        {
            // Assume
            Assume.That(_ellipseRendererComponentModel.RadiusX, Is.EqualTo(1));

            // Act
            _ellipseRendererComponentModel.RadiusX = 123;

            // Assert
            Assert.That(_ellipseRendererComponentModel.RadiusX, Is.EqualTo(123));
            Assert.That(_ellipseRendererComponent.RadiusX, Is.EqualTo(123));
        }

        [Test]
        public void RadiusY_ShouldUpdateEllipseRendererComponentRadiusY()
        {
            // Assume
            Assume.That(_ellipseRendererComponentModel.RadiusY, Is.EqualTo(2));

            // Act
            _ellipseRendererComponentModel.RadiusY = 123;

            // Assert
            Assert.That(_ellipseRendererComponentModel.RadiusY, Is.EqualTo(123));
            Assert.That(_ellipseRendererComponent.RadiusY, Is.EqualTo(123));
        }

        [Test]
        public void Color_ShouldUpdateEllipseRendererComponentColor()
        {
            // Assume
            Assume.That(_ellipseRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(1, 2, 3, 4)));

            // Act
            _ellipseRendererComponentModel.Color = Color.FromArgb(11, 22, 33, 44);

            // Assert
            Assert.That(_ellipseRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
            Assert.That(_ellipseRendererComponent.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
        }

        [Test]
        public void FillInterior_ShouldUpdateEllipseRendererComponentFillInterior()
        {
            // Assume
            Assume.That(_ellipseRendererComponentModel.FillInterior, Is.True);

            // Act
            _ellipseRendererComponentModel.FillInterior = false;

            // Assert
            Assert.That(_ellipseRendererComponentModel.FillInterior, Is.False);
            Assert.That(_ellipseRendererComponent.FillInterior, Is.False);
        }

        [Test]
        public void Visible_ShouldUpdateEllipseRendererComponentVisible()
        {
            // Assume
            Assume.That(_ellipseRendererComponentModel.Visible, Is.True);

            // Act
            _ellipseRendererComponentModel.Visible = false;

            // Assert
            Assert.That(_ellipseRendererComponentModel.Visible, Is.False);
            Assert.That(_ellipseRendererComponent.Visible, Is.False);
        }

        [Test]
        public void SortingLayerName_ShouldUpdateEllipseRendererComponentSortingLayerName()
        {
            // Assume
            Assume.That(_ellipseRendererComponentModel.SortingLayerName, Is.EqualTo("Test Layer"));

            // Act
            _ellipseRendererComponentModel.SortingLayerName = "Other Layer";

            // Assert
            Assert.That(_ellipseRendererComponentModel.SortingLayerName, Is.EqualTo("Other Layer"));
            Assert.That(_ellipseRendererComponent.SortingLayerName, Is.EqualTo("Other Layer"));
        }

        [Test]
        public void OrderInLayer_ShouldUpdateEllipseRendererComponentOrderInLayer()
        {
            // Assume
            Assume.That(_ellipseRendererComponentModel.OrderInLayer, Is.EqualTo(1));

            // Act
            _ellipseRendererComponentModel.OrderInLayer = 123;

            // Assert
            Assert.That(_ellipseRendererComponentModel.OrderInLayer, Is.EqualTo(123));
            Assert.That(_ellipseRendererComponent.OrderInLayer, Is.EqualTo(123));
        }
    }
}