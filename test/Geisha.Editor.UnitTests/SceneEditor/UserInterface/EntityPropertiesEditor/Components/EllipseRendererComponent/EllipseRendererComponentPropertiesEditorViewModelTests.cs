using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.EllipseRendererComponent;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.EllipseRendererComponent
{
    [TestFixture]
    public class EllipseRendererComponentPropertiesEditorViewModelTests
    {
        private EllipseRendererComponentModel _ellipseRendererComponentModel = null!;
        private EllipseRendererComponentPropertiesEditorViewModel _ellipseRendererComponentPropertiesEditorViewModel = null!;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var scene = TestSceneFactory.Create();
            var entity = scene.CreateEntity();

            var ellipseRendererComponent = entity.CreateComponent<Engine.Rendering.Components.EllipseRendererComponent>();
            ellipseRendererComponent.RadiusX = 1;
            ellipseRendererComponent.RadiusY = 2;
            ellipseRendererComponent.Color = Color.FromArgb(1, 2, 3, 4);
            ellipseRendererComponent.FillInterior = true;
            ellipseRendererComponent.Visible = true;
            ellipseRendererComponent.SortingLayerName = "Test Layer";
            ellipseRendererComponent.OrderInLayer = 1;

            _ellipseRendererComponentModel = new EllipseRendererComponentModel(ellipseRendererComponent);
            _ellipseRendererComponentPropertiesEditorViewModel = new EllipseRendererComponentPropertiesEditorViewModel(_ellipseRendererComponentModel);
        }

        [Test]
        public void RadiusX_ShouldUpdateEllipseRendererComponentModelRadiusX()
        {
            // Assume
            Assume.That(_ellipseRendererComponentPropertiesEditorViewModel.RadiusX, Is.EqualTo(1));

            // Act
            _ellipseRendererComponentPropertiesEditorViewModel.RadiusX = 123;

            // Assert
            Assert.That(_ellipseRendererComponentPropertiesEditorViewModel.RadiusX, Is.EqualTo(123));
            Assert.That(_ellipseRendererComponentModel.RadiusX, Is.EqualTo(123));
        }

        [Test]
        public void RadiusY_ShouldUpdateEllipseRendererComponentModelRadiusY()
        {
            // Assume
            Assume.That(_ellipseRendererComponentPropertiesEditorViewModel.RadiusY, Is.EqualTo(2));

            // Act
            _ellipseRendererComponentPropertiesEditorViewModel.RadiusY = 123;

            // Assert
            Assert.That(_ellipseRendererComponentPropertiesEditorViewModel.RadiusY, Is.EqualTo(123));
            Assert.That(_ellipseRendererComponentModel.RadiusY, Is.EqualTo(123));
        }

        [Test]
        public void Color_ShouldUpdateEllipseRendererComponentModelColor()
        {
            // Assume
            Assume.That(_ellipseRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(1, 2, 3, 4)));

            // Act
            _ellipseRendererComponentPropertiesEditorViewModel.Color = Color.FromArgb(11, 22, 33, 44);

            // Assert
            Assert.That(_ellipseRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
            Assert.That(_ellipseRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
        }

        [Test]
        public void FillInterior_ShouldUpdateEllipseRendererComponentModelFillInterior()
        {
            // Assume
            Assume.That(_ellipseRendererComponentPropertiesEditorViewModel.FillInterior, Is.True);

            // Act
            _ellipseRendererComponentPropertiesEditorViewModel.FillInterior = false;

            // Assert
            Assert.That(_ellipseRendererComponentPropertiesEditorViewModel.FillInterior, Is.False);
            Assert.That(_ellipseRendererComponentModel.FillInterior, Is.False);
        }

        [Test]
        public void Visible_ShouldUpdateEllipseRendererComponentModelVisible()
        {
            // Assume
            Assume.That(_ellipseRendererComponentPropertiesEditorViewModel.Visible, Is.True);

            // Act
            _ellipseRendererComponentPropertiesEditorViewModel.Visible = false;

            // Assert
            Assert.That(_ellipseRendererComponentPropertiesEditorViewModel.Visible, Is.False);
            Assert.That(_ellipseRendererComponentModel.Visible, Is.False);
        }

        [Test]
        public void SortingLayerName_ShouldUpdateEllipseRendererComponentModelSortingLayerName()
        {
            // Assume
            Assume.That(_ellipseRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Test Layer"));

            // Act
            _ellipseRendererComponentPropertiesEditorViewModel.SortingLayerName = "Other Layer";

            // Assert
            Assert.That(_ellipseRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Other Layer"));
            Assert.That(_ellipseRendererComponentModel.SortingLayerName, Is.EqualTo("Other Layer"));
        }

        [Test]
        public void OrderInLayer_ShouldUpdateEllipseRendererComponentModelOrderInLayer()
        {
            // Assume
            Assume.That(_ellipseRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(1));

            // Act
            _ellipseRendererComponentPropertiesEditorViewModel.OrderInLayer = 123;

            // Assert
            Assert.That(_ellipseRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(123));
            Assert.That(_ellipseRendererComponentModel.OrderInLayer, Is.EqualTo(123));
        }
    }
}