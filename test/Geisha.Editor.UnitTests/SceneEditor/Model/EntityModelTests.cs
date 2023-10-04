using System;
using System.Diagnostics;
using System.Linq;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.Model
{
    [TestFixture]
    public class EntityModelTests
    {
        private Scene Scene { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            Scene = TestSceneFactory.Create(new[] { new UnsupportedComponentFactory() });
        }

        #region Constructor tests

        [Test]
        public void Constructor_ShouldThrowException_GivenEntityWithNotSupportedComponentType()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<UnsupportedComponent>();

            // Act
            // Assert
            Assert.That(() => new EntityModel(entity), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Constructor_ShouldCreateEntityModelWithTransform3DComponent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<Transform3DComponent>();

            // Act
            var entityModel = new EntityModel(entity);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Transform 3D Component"));
        }

        [Test]
        public void Constructor_ShouldCreateEntityModelWithEllipseRendererComponent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<EllipseRendererComponent>();

            // Act
            var entityModel = new EntityModel(entity);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Ellipse Renderer Component"));
        }

        [Test]
        public void Constructor_ShouldCreateEntityModelWithRectangleRendererComponent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<RectangleRendererComponent>();

            // Act
            var entityModel = new EntityModel(entity);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Rectangle Renderer Component"));
        }

        [Test]
        public void Constructor_ShouldCreateEntityModelWithTextRendererComponent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<TextRendererComponent>();

            // Act
            var entityModel = new EntityModel(entity);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Text Renderer Component"));
        }

        [Test]
        public void Constructor_ShouldCreateEntityModelWithCircleColliderComponent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<CircleColliderComponent>();

            // Act
            var entityModel = new EntityModel(entity);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Circle Collider Component"));
        }

        [Test]
        public void Constructor_ShouldCreateEntityModelWithRectangleColliderComponent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<RectangleColliderComponent>();

            // Act
            var entityModel = new EntityModel(entity);

            // Assert
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components.Single().Name, Is.EqualTo("Rectangle Collider Component"));
        }

        #endregion

        [Test]
        public void Name_ShouldUpdateEntityNameAndNotifyWithEvent_WhenChanged()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.Name = "Old name";
            var entityModel = new EntityModel(entity);

            object? eventSender = null;
            PropertyChangedEventArgs<string>? eventArgs = null;
            entityModel.NameChanged += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.Name = "New name";

            // Assert
            Assert.That(entity.Name, Is.EqualTo("New name"));
            Assert.That(entityModel.Name, Is.EqualTo("New name"));
            Assert.That(eventSender, Is.EqualTo(entityModel));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.OldValue, Is.EqualTo("Old name"));
            Assert.That(eventArgs.NewValue, Is.EqualTo("New name"));
        }

        [Test]
        public void AddChildEntity_ShouldAddNewChildEntityAndNotifyWithEvent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);

            object? eventSender = null;
            EntityAddedEventArgs? eventArgs = null;
            entityModel.EntityAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.AddChildEntity();

            // Assert
            Assert.That(entity.Children, Has.Count.EqualTo(1));
            Assert.That(entityModel.Children, Has.Count.EqualTo(1));

            var childEntity = entity.Children.Single();
            var childEntityModel = entityModel.Children.Single();
            Assert.That(childEntity.Name, Is.EqualTo("Child entity 1"));
            Assert.That(childEntityModel.Name, Is.EqualTo("Child entity 1"));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.EntityModel, Is.EqualTo(childEntityModel));
        }

        [Test]
        public void AddChildEntity_ShouldAddChildEntitiesWithIncrementingDefaultNames_WhenEntityInitiallyHasNoChildren()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);

            // Act
            entityModel.AddChildEntity();
            entityModel.AddChildEntity();
            entityModel.AddChildEntity();

            // Assert
            Assert.That(entityModel.Children.Select(e => e.Name), Is.EquivalentTo(new[] { "Child entity 1", "Child entity 2", "Child entity 3" }));
        }

        [Test]
        public void AddTransform3DComponent_ShouldAddTransform3DComponentAndNotifyWithEvent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);

            object? eventSender = null;
            ComponentAddedEventArgs? eventArgs = null;
            entityModel.ComponentAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.AddTransform3DComponent();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));

            var transformComponent = entity.Components.Single();
            var transformComponentModel = entityModel.Components.Single();
            Assert.That(transformComponent, Is.TypeOf<Transform3DComponent>());
            Assert.That(transformComponentModel, Is.TypeOf<Transform3DComponentModel>());

            // Assert that created component model is bound to component
            ((Transform3DComponentModel)transformComponentModel).Translation = new Vector3(123, 456, 789);
            Assert.That(((Transform3DComponent)transformComponent).Translation, Is.EqualTo(new Vector3(123, 456, 789)));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.ComponentModel, Is.EqualTo(transformComponentModel));
        }

        [Test]
        public void AddTransform3DComponent_ShouldAddTransform3DComponentWithDefaultValues()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);

            // Act
            entityModel.AddTransform3DComponent();

            // Assert
            var transformComponentModel = (Transform3DComponentModel)entityModel.Components.Single();
            Assert.That(transformComponentModel.Translation, Is.EqualTo(Vector3.Zero));
            Assert.That(transformComponentModel.Rotation, Is.EqualTo(Vector3.Zero));
            Assert.That(transformComponentModel.Scale, Is.EqualTo(Vector3.One));
        }

        [Test]
        public void AddEllipseRendererComponent_ShouldAddEllipseRendererComponentAndNotifyWithEvent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);

            object? eventSender = null;
            ComponentAddedEventArgs? eventArgs = null;
            entityModel.ComponentAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.AddEllipseRendererComponent();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));

            var ellipseRendererComponent = entity.Components.Single();
            var ellipseRendererComponentModel = entityModel.Components.Single();
            Assert.That(ellipseRendererComponent, Is.TypeOf<EllipseRendererComponent>());
            Assert.That(ellipseRendererComponentModel, Is.TypeOf<EllipseRendererComponentModel>());

            // Assert that created component model is bound to component
            ((EllipseRendererComponentModel)ellipseRendererComponentModel).RadiusX = 123;
            Assert.That(((EllipseRendererComponent)ellipseRendererComponent).RadiusX, Is.EqualTo(123));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.ComponentModel, Is.EqualTo(ellipseRendererComponentModel));
        }

        [Test]
        public void AddRectangleRendererComponent_ShouldAddRectangleRendererComponentAndNotifyWithEvent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);

            object? eventSender = null;
            ComponentAddedEventArgs? eventArgs = null;
            entityModel.ComponentAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.AddRectangleRendererComponent();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));

            var rectangleRendererComponent = entity.Components.Single();
            var rectangleRendererComponentModel = entityModel.Components.Single();
            Assert.That(rectangleRendererComponent, Is.TypeOf<RectangleRendererComponent>());
            Assert.That(rectangleRendererComponentModel, Is.TypeOf<RectangleRendererComponentModel>());

            // Assert that created component model is bound to component
            ((RectangleRendererComponentModel)rectangleRendererComponentModel).Dimensions = new Vector2(123, 456);
            Assert.That(((RectangleRendererComponent)rectangleRendererComponent).Dimensions, Is.EqualTo(new Vector2(123, 456)));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.ComponentModel, Is.EqualTo(rectangleRendererComponentModel));
        }

        [Test]
        public void AddTextRendererComponent_ShouldAddTextRendererComponentAndNotifyWithEvent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);

            object? eventSender = null;
            ComponentAddedEventArgs? eventArgs = null;
            entityModel.ComponentAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.AddTextRendererComponent();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));

            var textRendererComponent = entity.Components.Single();
            var textRendererComponentModel = entityModel.Components.Single();
            Assert.That(textRendererComponent, Is.TypeOf<TextRendererComponent>());
            Assert.That(textRendererComponentModel, Is.TypeOf<TextRendererComponentModel>());

            // Assert that created component model is bound to component
            ((TextRendererComponentModel)textRendererComponentModel).Text = "Some text";
            Assert.That(((TextRendererComponent)textRendererComponent).Text, Is.EqualTo("Some text"));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.ComponentModel, Is.EqualTo(textRendererComponentModel));
        }

        [Test]
        public void AddCircleColliderComponent_ShouldAddCircleColliderComponentAndNotifyWithEvent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);

            object? eventSender = null;
            ComponentAddedEventArgs? eventArgs = null;
            entityModel.ComponentAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.AddCircleColliderComponent();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));

            var circleColliderComponent = entity.Components.Single();
            var circleColliderComponentModel = entityModel.Components.Single();
            Assert.That(circleColliderComponent, Is.TypeOf<CircleColliderComponent>());
            Assert.That(circleColliderComponentModel, Is.TypeOf<CircleColliderComponentModel>());

            // Assert that created component model is bound to component
            ((CircleColliderComponentModel)circleColliderComponentModel).Radius = 123;
            Assert.That(((CircleColliderComponent)circleColliderComponent).Radius, Is.EqualTo(123));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.ComponentModel, Is.EqualTo(circleColliderComponentModel));
        }

        [Test]
        public void AddRectangleColliderComponent_ShouldAddRectangleColliderComponentAndNotifyWithEvent()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var entityModel = new EntityModel(entity);

            object? eventSender = null;
            ComponentAddedEventArgs? eventArgs = null;
            entityModel.ComponentAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            entityModel.AddRectangleColliderComponent();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(1));
            Assert.That(entityModel.Components, Has.Count.EqualTo(1));

            var rectangleColliderComponent = entity.Components.Single();
            var rectangleColliderComponentModel = entityModel.Components.Single();
            Assert.That(rectangleColliderComponent, Is.TypeOf<RectangleColliderComponent>());
            Assert.That(rectangleColliderComponentModel, Is.TypeOf<RectangleColliderComponentModel>());

            // Assert that created component model is bound to component
            ((RectangleColliderComponentModel)rectangleColliderComponentModel).Dimensions = new Vector2(123, 456);
            Assert.That(((RectangleColliderComponent)rectangleColliderComponent).Dimensions, Is.EqualTo(new Vector2(123, 456)));

            Assert.That(eventSender, Is.EqualTo(entityModel));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.ComponentModel, Is.EqualTo(rectangleColliderComponentModel));
        }

        #region Helpers

        private sealed class UnsupportedComponent : Component
        {
            public UnsupportedComponent(Entity entity) : base(entity)
            {
            }
        }

        private sealed class UnsupportedComponentFactory : ComponentFactory<UnsupportedComponent>
        {
            protected override UnsupportedComponent CreateComponent(Entity entity) => new UnsupportedComponent(entity);
        }

        #endregion
    }
}