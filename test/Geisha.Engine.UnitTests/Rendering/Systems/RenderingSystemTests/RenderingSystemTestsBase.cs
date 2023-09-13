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
            transform2DComponent.Translation = translation;
            transform2DComponent.Rotation = rotation;
            transform2DComponent.Scale = scale;

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

        public (Entity entity, TextRendererComponent textRendererComponent) AddText()
        {
            var entity = _scene.CreateEntity();

            var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
            SetTransformInCameraView(transform2DComponent);

            var textRendererComponent = entity.CreateComponent<TextRendererComponent>();

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

        public Entity AddEllipse()
        {
            var entity = _scene.CreateEntity();
            CreateEllipse(entity);
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
            transform2DComponent.Translation = new Vector2(100, 200);
            transform2DComponent.Rotation = Angle.Deg2Rad(30);
            transform2DComponent.Scale = new Vector2(0.5, 0.5);
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