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
        public void Dimension_ShouldUpdateRectangleColliderComponentModelDimension()
        {
            // Assume
            Assume.That(_rectangleColliderComponentPropertiesEditorViewModel.Dimension, Is.EqualTo(new Vector2(1, 2)));

            // Act
            _rectangleColliderComponentPropertiesEditorViewModel.Dimension = new Vector2(123, 456);

            // Assert
            Assert.That(_rectangleColliderComponentPropertiesEditorViewModel.Dimension, Is.EqualTo(new Vector2(123, 456)));
            Assert.That(_rectangleColliderComponentModel.Dimension, Is.EqualTo(new Vector2(123, 456)));
        }
    }
}