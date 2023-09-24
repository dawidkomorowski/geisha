using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Diagnostics;
using Geisha.Engine.Rendering.Systems;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

public abstract class RenderingSystemTestsBase
{
    protected const int ScreenWidth = 2000;
    protected const int ScreenHeight = 1000;
    protected IRenderingContext2D RenderingContext2D = null!;
    protected IRenderingBackend RenderingBackend = null!;
    protected IAggregatedDiagnosticInfoProvider AggregatedDiagnosticInfoProvider = null!;
    private protected IDebugRendererForRenderingSystem DebugRendererForRenderingSystem = null!;
    private protected IRenderingDiagnosticInfoProvider RenderingDiagnosticInfoProvider = null!;

    [SetUp]
    public void SetUp()
    {
        RenderingContext2D = Substitute.For<IRenderingContext2D>();
        RenderingContext2D.ScreenWidth.Returns(ScreenWidth);
        RenderingContext2D.ScreenHeight.Returns(ScreenHeight);

        RenderingBackend = Substitute.For<IRenderingBackend>();
        RenderingBackend.Context2D.Returns(RenderingContext2D);
        AggregatedDiagnosticInfoProvider = Substitute.For<IAggregatedDiagnosticInfoProvider>();
        DebugRendererForRenderingSystem = Substitute.For<IDebugRendererForRenderingSystem>();
        RenderingDiagnosticInfoProvider = Substitute.For<IRenderingDiagnosticInfoProvider>();
    }

    private protected (RenderingSystem renderingSystem, RenderingScene renderingScene) GetRenderingSystem()
    {
        return GetRenderingSystem(new RenderingConfiguration());
    }

    private protected (RenderingSystem renderingSystem, RenderingScene renderingScene) GetRenderingSystem(RenderingConfiguration configuration)
    {
        var renderingSystem = new RenderingSystem(
            RenderingBackend,
            configuration,
            AggregatedDiagnosticInfoProvider,
            DebugRendererForRenderingSystem,
            RenderingDiagnosticInfoProvider
        );

        var renderingScene = new RenderingScene(renderingSystem);

        return (renderingSystem, renderingScene);
    }

    protected static DiagnosticInfo GetRandomDiagnosticInfo()
    {
        return new DiagnosticInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
    }

    protected sealed class RenderingScene
    {
        private readonly Scene _scene = TestSceneFactory.Create();

        public RenderingScene(ISceneObserver observer)
        {
            _scene.AddObserver(observer);
        }

        public Scene Scene => _scene;

        public Entity AddCamera()
        {
            var entity = _scene.CreateEntity();
            entity.CreateComponent<Transform2DComponent>();

            var cameraComponent = entity.CreateComponent<CameraComponent>();
            cameraComponent.ViewRectangle = new Vector2(ScreenWidth, ScreenHeight);

            return entity;
        }

        public Entity AddCamera(Vector2 translation, double rotation, Vector2 scale)
        {
            var entity = AddCamera();
            var transform2DComponent = entity.GetComponent<Transform2DComponent>();
            SetTransform(transform2DComponent, translation, rotation, scale);

            return entity;
        }

        public Entity AddSpriteWithDefaultTransform()
        {
            var entity = _scene.CreateEntity();
            entity.CreateComponent<Transform2DComponent>();

            var spriteRendererComponent = entity.CreateComponent<SpriteRendererComponent>();
            spriteRendererComponent.Sprite = new Sprite(Substitute.For<ITexture>(), Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);

            return entity;
        }

        public Entity AddSprite(int orderInLayer = 0, string sortingLayerName = RenderingConfiguration.DefaultSortingLayerName, bool visible = true)
        {
            var entity = AddSpriteWithDefaultTransform();

            var transformComponent = entity.GetComponent<Transform2DComponent>();
            SetTransformInCameraView(transformComponent);

            var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();
            spriteRendererComponent.OrderInLayer = orderInLayer;
            spriteRendererComponent.SortingLayerName = sortingLayerName;
            spriteRendererComponent.Visible = visible;

            return entity;
        }

        public Entity AddSprite(Vector2 dimensions, Vector2 translation, double rotation, Vector2 scale)
        {
            var entity = AddSprite();

            var transform2DComponent = entity.GetComponent<Transform2DComponent>();
            SetTransform(transform2DComponent, translation, rotation, scale);

            var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();
            spriteRendererComponent.Sprite = new Sprite(Substitute.For<ITexture>(), Vector2.Zero, dimensions, dimensions / 2, 1);

            return entity;
        }

        public (Entity entity, TextRendererComponent textRendererComponent) AddText()
        {
            var entity = _scene.CreateEntity();

            var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
            SetTransformInCameraView(transform2DComponent);

            var textRendererComponent = entity.CreateComponent<TextRendererComponent>();

            return (entity, textRendererComponent);
        }

        public (Entity entity, TextRendererComponent textRendererComponent) AddText(Vector2 translation, double rotation, Vector2 scale)
        {
            var (entity, textRendererComponent) = AddText();

            var transform2DComponent = entity.GetComponent<Transform2DComponent>();
            SetTransform(transform2DComponent, translation, rotation, scale);

            return (entity, textRendererComponent);
        }

        public Entity AddRectangle()
        {
            var entity = _scene.CreateEntity();

            var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
            SetTransformInCameraView(transform2DComponent);

            var rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();
            rectangleRendererComponent.Dimension = Utils.RandomVector2();
            rectangleRendererComponent.Color = Color.FromArgb(Utils.Random.Next());
            rectangleRendererComponent.FillInterior = Utils.Random.NextBool();

            return entity;
        }

        public Entity AddRectangle(Vector2 dimensions, Vector2 translation, double rotation, Vector2 scale)
        {
            var entity = AddRectangle();

            var transform2DComponent = entity.GetComponent<Transform2DComponent>();
            SetTransform(transform2DComponent, translation, rotation, scale);

            var rectangleRendererComponent = entity.GetComponent<RectangleRendererComponent>();
            rectangleRendererComponent.Dimension = dimensions;

            return entity;
        }

        public Entity AddEllipse()
        {
            var entity = _scene.CreateEntity();
            CreateEllipse(entity);
            return entity;
        }

        public Entity AddEllipse(double radiusX, double radiusY, Vector2 translation, double rotation, Vector2 scale)
        {
            var entity = AddEllipse();

            var transform2DComponent = entity.GetComponent<Transform2DComponent>();
            SetTransform(transform2DComponent, translation, rotation, scale);

            var ellipseRendererComponent = entity.GetComponent<EllipseRendererComponent>();
            ellipseRendererComponent.RadiusX = radiusX;
            ellipseRendererComponent.RadiusY = radiusY;

            return entity;
        }

        public (Entity parent, Entity child) AddParentEllipseWithChildEllipse()
        {
            var parent = _scene.CreateEntity();
            CreateEllipse(parent);

            var child = parent.CreateChildEntity();
            CreateEllipse(child);

            return (parent, child);
        }

        private static void SetTransformInCameraView(Transform2DComponent transform2DComponent)
        {
            SetTransform(transform2DComponent, new Vector2(100, 200), Angle.Deg2Rad(30), new Vector2(0.5, 0.5));
        }

        private static void SetTransform(Transform2DComponent transform2DComponent, Vector2 translation, double rotation, Vector2 scale)
        {
            transform2DComponent.Translation = translation;
            transform2DComponent.Rotation = rotation;
            transform2DComponent.Scale = scale;
        }

        private static void CreateEllipse(Entity entity)
        {
            var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
            SetTransformInCameraView(transform2DComponent);

            var ellipseRendererComponent = entity.CreateComponent<EllipseRendererComponent>();
            ellipseRendererComponent.RadiusX = Utils.Random.NextDouble();
            ellipseRendererComponent.RadiusY = Utils.Random.NextDouble();
            ellipseRendererComponent.Color = Color.FromArgb(Utils.Random.Next());
            ellipseRendererComponent.FillInterior = Utils.Random.NextBool();
        }
    }
}