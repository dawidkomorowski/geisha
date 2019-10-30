using System;
using System.Threading;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.CircleColliderComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components
{
    [TestFixture]
    public class ComponentPropertiesEditorViewModelFactoryTests
    {
        private ComponentPropertiesEditorViewModelFactory _componentPropertiesEditorViewModelFactory;

        [SetUp]
        public void SetUp()
        {
            _componentPropertiesEditorViewModelFactory = new ComponentPropertiesEditorViewModelFactory();
        }

        [Test]
        public void Create_ShouldThrowException_GivenUnsupportedComponentModel()
        {
            // Arrange
            var componentModel = Substitute.For<IComponentModel>();

            // Act
            // Assert
            Assert.That(() => _componentPropertiesEditorViewModelFactory.Create(componentModel), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Create_ShouldCreateTransformComponentPropertiesEditorViewModel_GivenTransformComponentModel()
        {
            // Arrange
            var componentModel = new TransformComponentModel(new Engine.Core.Components.TransformComponent());

            // Act
            var viewModel = _componentPropertiesEditorViewModelFactory.Create(componentModel);

            // Assert
            Assert.That(viewModel, Is.TypeOf<TransformComponentPropertiesEditorViewModel>());
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Create_ShouldCreateCircleColliderComponentPropertiesEditorViewModel_GivenCircleColliderComponentModel()
        {
            // Arrange
            var componentModel = new CircleColliderComponentModel(new Engine.Physics.Components.CircleColliderComponent());

            // Act
            var viewModel = _componentPropertiesEditorViewModelFactory.Create(componentModel);

            // Assert
            Assert.That(viewModel, Is.TypeOf<CircleColliderComponentPropertiesEditorViewModel>());
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Create_ShouldCreateRectangleColliderComponentPropertiesEditorViewModel_GivenRectangleColliderComponentModel()
        {
            // Arrange
            var componentModel = new RectangleColliderComponentModel(new Engine.Physics.Components.RectangleColliderComponent());

            // Act
            var viewModel = _componentPropertiesEditorViewModelFactory.Create(componentModel);

            // Assert
            Assert.That(viewModel, Is.TypeOf<RectangleColliderComponentPropertiesEditorViewModel>());
        }
    }
}