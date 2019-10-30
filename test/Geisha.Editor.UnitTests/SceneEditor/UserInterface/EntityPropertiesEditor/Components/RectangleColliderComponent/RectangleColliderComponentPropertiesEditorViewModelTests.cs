using System.Threading;
using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent
{
    [TestFixture]
    public class RectangleColliderComponentPropertiesEditorViewModelTests
    {
        private RectangleColliderComponentModel _rectangleColliderComponentModel;
        private RectangleColliderComponentPropertiesEditorViewModel _rectangleColliderComponentPropertiesEditorViewModel;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var rectangleColliderComponent = new Engine.Physics.Components.RectangleColliderComponent {Dimension = new Vector2(1, 2)};
            _rectangleColliderComponentModel = new RectangleColliderComponentModel(rectangleColliderComponent);
            _rectangleColliderComponentPropertiesEditorViewModel = new RectangleColliderComponentPropertiesEditorViewModel(_rectangleColliderComponentModel);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void DimensionX_ShouldUpdateRectangleColliderComponentModelDimensionX()
        {
            // Assume
            Assume.That(_rectangleColliderComponentPropertiesEditorViewModel.DimensionX, Is.EqualTo(1));

            // Act
            _rectangleColliderComponentPropertiesEditorViewModel.DimensionX = 123;

            // Assert
            Assert.That(_rectangleColliderComponentPropertiesEditorViewModel.DimensionX, Is.EqualTo(123));
            Assert.That(_rectangleColliderComponentModel.DimensionX, Is.EqualTo(123));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void DimensionY_ShouldUpdateRectangleColliderComponentModelDimensionY()
        {
            // Assume
            Assume.That(_rectangleColliderComponentPropertiesEditorViewModel.DimensionY, Is.EqualTo(2));

            // Act
            _rectangleColliderComponentPropertiesEditorViewModel.DimensionY = 123;

            // Assert
            Assert.That(_rectangleColliderComponentPropertiesEditorViewModel.DimensionY, Is.EqualTo(123));
            Assert.That(_rectangleColliderComponentModel.DimensionY, Is.EqualTo(123));
        }
    }
}