using System;
using Geisha.Common.Math;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Common
{
    internal interface IEntityFactory
    {
        Entity CreateCamera(Scene scene);
        Entity CreateStaticSprite(Scene scene, double x, double y);
        Entity CreateMovingSprite(Scene scene, double x, double y, Random random);
        Entity CreateAnimatedSprite(Scene scene, double x, double y, Random random);
        Entity CreateMovingCircleCollider(Scene scene, double x, double y, Random random);
        Entity CreateMovingRectangleCollider(Scene scene, double x, double y, Random random);
        Entity CreateTurret(Scene scene, double x, double y, Random random);
    }

    internal sealed class EntityFactory : IEntityFactory
    {
        private readonly IAssetStore _assetStore;

        public EntityFactory(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        public Entity CreateCamera(Scene scene)
        {
            var entity = scene.CreateEntity();

            entity.CreateComponent<Transform2DComponent>();

            var camera = entity.CreateComponent<CameraComponent>();
            camera.ViewRectangle = new Vector2(1280, 720);

            return entity;
        }

        public Entity CreateStaticSprite(Scene scene, double x, double y)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            var spriteRenderer = entity.CreateComponent<SpriteRendererComponent>();
            spriteRenderer.Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.PaintColorPalette);

            return entity;
        }

        public Entity CreateMovingSprite(Scene scene, double x, double y, Random random)
        {
            var entity = CreateStaticSprite(scene, x, y);

            var movementBehavior = entity.CreateComponent<MovementBehaviorComponent>();
            movementBehavior.RandomFactor = random.NextDouble() * 10;

            return entity;
        }

        public Entity CreateAnimatedSprite(Scene scene, double x, double y, Random random)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            entity.CreateComponent<SpriteRendererComponent>();

            var spriteAnimationComponent = entity.CreateComponent<SpriteAnimationComponent>();

            spriteAnimationComponent.AddAnimation("Explosion", _assetStore.GetAsset<SpriteAnimation>(AssetsIds.ExplosionAnimation));
            spriteAnimationComponent.PlayAnimation("Explosion");
            spriteAnimationComponent.Position = random.NextDouble();
            spriteAnimationComponent.PlayInLoop = true;

            return entity;
        }

        public Entity CreateMovingCircleCollider(Scene scene, double x, double y, Random random)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            var movementBehavior = entity.CreateComponent<MovementBehaviorComponent>();
            movementBehavior.RandomFactor = random.NextDouble();

            var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
            circleColliderComponent.Radius = 50;

            return entity;
        }

        public Entity CreateMovingRectangleCollider(Scene scene, double x, double y, Random random)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            var movementBehavior = entity.CreateComponent<MovementBehaviorComponent>();
            movementBehavior.RandomFactor = random.NextDouble();

            var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
            rectangleColliderComponent.Dimension = new Vector2(100, 50);

            return entity;
        }

        public Entity CreateTurret(Scene scene, double x, double y, Random random)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            var rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();
            rectangleRendererComponent.Color = Color.FromArgb(1, random.NextDouble(), random.NextDouble(), random.NextDouble());
            rectangleRendererComponent.Dimension = new Vector2(30, 30);
            rectangleRendererComponent.FillInterior = true;

            CreateCannon(entity, random);

            return entity;
        }

        private Entity CreateCannon(Entity entity, Random random)
        {
            var cannonRotor = entity.CreateChildEntity();

            var canonRotorTransform = cannonRotor.CreateComponent<Transform2DComponent>();
            canonRotorTransform.Rotation = random.NextDouble();

            var cannon = cannonRotor.CreateChildEntity();

            var cannonTransform = cannon.CreateComponent<Transform2DComponent>();
            cannonTransform.Translation = new Vector2(0, 10);

            var rectangleRenderer = cannon.CreateComponent<RectangleRendererComponent>();
            rectangleRenderer.Color = Color.FromArgb(1, random.NextDouble(), random.NextDouble(), random.NextDouble());
            rectangleRenderer.Dimension = new Vector2(10, 30);
            rectangleRenderer.FillInterior = true;
            rectangleRenderer.OrderInLayer = 1;

            var cannonBehavior = cannon.CreateComponent<CannonBehaviorComponent>();
            cannonBehavior.Random = random;

            return cannon;
        }
    }
}