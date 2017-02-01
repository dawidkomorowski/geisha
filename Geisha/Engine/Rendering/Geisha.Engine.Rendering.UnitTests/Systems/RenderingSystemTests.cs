using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Systems;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Systems
{
    [TestFixture]
    public class RenderingSystemTests
    {
        private const double DeltaTime = 0.1;
        private IRenderer2D _renderer2D;
        private RenderingSystem _renderingSystem;

        [SetUp]
        public void SetUp()
        {
            _renderer2D = Substitute.For<IRenderer2D>();
            _renderingSystem = new RenderingSystem(_renderer2D);
        }

        [Test]
        public void Update_ShouldClearOnce()
        {
            // Arrange
            var scene = new RootOnlyScene();
            _renderingSystem.Scene = scene;

            // Act
            _renderingSystem.Update(DeltaTime);

            // Assert
            _renderer2D.Received(1).Clear();
        }

        [Test]
        public void Update_ShouldRenderTextureOnce_WhenSceneContainsEntityWithSpriteRendererAndTransform()
        {
            // Arrange
            var scene = new SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform();
            _renderingSystem.Scene = scene;

            // Act
            _renderingSystem.Update(DeltaTime);

            // Assert
            var sprite = scene.EntityWithSpriteRendererAndTransform.GetComponent<SpriteRenderer>().Sprite;
            var position = scene.EntityWithSpriteRendererAndTransform.GetComponent<Transform>().Translation.ToVector2();
            _renderer2D.Received(1).Render(sprite, Matrix3.Identity);
        }

        [Test]
        public void Update_ShouldFirstClearThenRender()
        {
            // Arrange
            var scene = new SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform();
            _renderingSystem.Scene = scene;

            // Act
            _renderingSystem.Update(DeltaTime);

            // Assert
            Received.InOrder((() =>
            {
                _renderer2D.Clear();
                _renderer2D.Render(Arg.Any<Sprite>(), Arg.Any<Matrix3>());
            }));
        }

        [Test]
        public void FixedUpdate_ShouldClearOnce()
        {
            // Arrange
            var scene = new RootOnlyScene();
            _renderingSystem.Scene = scene;

            // Act
            _renderingSystem.FixedUpdate();

            // Assert
            _renderer2D.Received(1).Clear();
        }

        [Test]
        public void FixedUpdate_ShouldRenderTextureOnce_WhenSceneContainsEntityWithSpriteRendererAndTransform()
        {
            // Arrange
            var scene = new SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform();
            _renderingSystem.Scene = scene;

            // Act
            _renderingSystem.FixedUpdate();

            // Assert
            var sprite = scene.EntityWithSpriteRendererAndTransform.GetComponent<SpriteRenderer>().Sprite;
            var position = scene.EntityWithSpriteRendererAndTransform.GetComponent<Transform>().Translation.ToVector2();
            _renderer2D.Received(1).Render(sprite, Matrix3.Identity);
        }

        [Test]
        public void FixedUpdate_ShouldFirstClearThenRender()
        {
            // Arrange
            var scene = new SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform();
            _renderingSystem.Scene = scene;

            // Act
            _renderingSystem.FixedUpdate();

            // Assert
            Received.InOrder((() =>
            {
                _renderer2D.Clear();
                _renderer2D.Render(Arg.Any<Sprite>(), Arg.Any<Matrix3>());
            }));
        }

        private class RootOnlyScene : Scene
        {
            public RootOnlyScene()
            {
                RootEntity = new Entity();
            }
        }

        private class SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform : Scene
        {
            public Entity EntityWithSpriteRendererAndTransform { get; }

            public SceneWithTwoEntitiesButOnlyOneWithSpriteRendererAndTransform()
            {
                RootEntity = new Entity();

                EntityWithSpriteRendererAndTransform = new Entity {Parent = RootEntity};
                var spriteRenderer = new SpriteRenderer {Sprite = new Sprite()};
                EntityWithSpriteRendererAndTransform.AddComponent(spriteRenderer);
                var transform = new Transform {Translation = new Vector3(1, 2, 3)};
                EntityWithSpriteRendererAndTransform.AddComponent(transform);
            }
        }
    }
}