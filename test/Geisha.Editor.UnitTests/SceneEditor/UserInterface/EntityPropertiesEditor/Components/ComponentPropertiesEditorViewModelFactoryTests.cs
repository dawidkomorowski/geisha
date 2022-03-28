using System;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.CircleColliderComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.EllipseRendererComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleRendererComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TextRendererComponent;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.Transform3DComponent;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components
{
    [TestFixture]
    public class ComponentPropertiesEditorViewModelFactoryTests
    {
        private ComponentPropertiesEditorViewModelFactory _componentPropertiesEditorViewModelFactory = null!;
        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            _componentPropertiesEditorViewModelFactory = new ComponentPropertiesEditorViewModelFactory();

            var scene = TestSceneFactory.Create();
            Entity = scene.CreateEntity();
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
        public void Create_ShouldCreateTransform3DComponentPropertiesEditorViewModel_GivenTransform3DComponentModel()
        {
            // Arrange
            var componentModel = new Transform3DComponentModel(Entity.CreateComponent<Engine.Core.Components.Transform3DComponent>());

            // Act
            var viewModel = _componentPropertiesEditorViewModelFactory.Create(componentModel);

            // Assert
            Assert.That(viewModel, Is.TypeOf<Transform3DComponentPropertiesEditorViewModel>());
        }

        [Test]
        public void Create_ShouldCreateEllipseRendererComponentPropertiesEditorViewModel_GivenEllipseRendererComponentModel()
        {
            // Arrange
            var componentModel = new EllipseRendererComponentModel(Entity.CreateComponent<Engine.Rendering.Components.EllipseRendererComponent>());

            // Act
            var viewModel = _componentPropertiesEditorViewModelFactory.Create(componentModel);

            // Assert
            Assert.That(viewModel, Is.TypeOf<EllipseRendererComponentPropertiesEditorViewModel>());
        }

        [Test]
        public void Create_ShouldCreateRectangleRendererComponentPropertiesEditorViewModel_GivenRectangleRendererComponentModel()
        {
            // Arrange
            var componentModel = new RectangleRendererComponentModel(Entity.CreateComponent<Engine.Rendering.Components.RectangleRendererComponent>());

            // Act
            var viewModel = _componentPropertiesEditorViewModelFactory.Create(componentModel);

            // Assert
            Assert.That(viewModel, Is.TypeOf<RectangleRendererComponentPropertiesEditorViewModel>());
        }

        [Test]
        public void Create_ShouldCreateTextRendererComponentPropertiesEditorViewModel_GivenTextRendererComponentModel()
        {
            // Arrange
            var componentModel = new TextRendererComponentModel(Entity.CreateComponent<Engine.Rendering.Components.TextRendererComponent>());

            // Act
            var viewModel = _componentPropertiesEditorViewModelFactory.Create(componentModel);

            // Assert
            Assert.That(viewModel, Is.TypeOf<TextRendererComponentPropertiesEditorViewModel>());
        }

        [Test]
        public void Create_ShouldCreateCircleColliderComponentPropertiesEditorViewModel_GivenCircleColliderComponentModel()
        {
            // Arrange
            var componentModel = new CircleColliderComponentModel(Entity.CreateComponent<Engine.Physics.Components.CircleColliderComponent>());

            // Act
            var viewModel = _componentPropertiesEditorViewModelFactory.Create(componentModel);

            // Assert
            Assert.That(viewModel, Is.TypeOf<CircleColliderComponentPropertiesEditorViewModel>());
        }

        [Test]
        public void Create_ShouldCreateRectangleColliderComponentPropertiesEditorViewModel_GivenRectangleColliderComponentModel()
        {
            // Arrange
            var componentModel = new RectangleColliderComponentModel(Entity.CreateComponent<Engine.Physics.Components.RectangleColliderComponent>());

            // Act
            var viewModel = _componentPropertiesEditorViewModelFactory.Create(componentModel);

            // Assert
            Assert.That(viewModel, Is.TypeOf<RectangleColliderComponentPropertiesEditorViewModel>());
        }
    }
}