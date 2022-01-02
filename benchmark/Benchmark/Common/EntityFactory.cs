using Geisha.Common.Math;
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
        Entity CreateMovingSprite(Scene scene, double x, double y, double randomFactor = 0);
        Entity CreateMovingCircleCollider(Scene scene, double x, double y, double randomFactor = 0);
        Entity CreateMovingRectangleCollider(Scene scene, double x, double y, double randomFactor = 0);
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
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.AddComponent(new Transform2DComponent
            {
                Translation = Vector2.Zero,
                Rotation = 0,
                Scale = Vector2.One
            });
            entity.AddComponent(new CameraComponent
            {
                ViewRectangle = new Vector2(1280, 720)
            });

            return entity;
        }

        public Entity CreateStaticSprite(Scene scene, double x, double y)
        {
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(x, y),
                Rotation = 0,
                Scale = Vector2.One
            });
            entity.AddComponent(new SpriteRendererComponent
            {
                Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.PaintColorPalette)
            });

            return entity;
        }

        public Entity CreateMovingSprite(Scene scene, double x, double y, double randomFactor = 0)
        {
            var entity = CreateStaticSprite(scene, x, y);
            entity.AddComponent(new MovementBehavior
            {
                RandomFactor = randomFactor
            });

            return entity;
        }

        public Entity CreateMovingCircleCollider(Scene scene, double x, double y, double randomFactor = 0)
        {
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(x, y),
                Rotation = 0,
                Scale = Vector2.One
            });
            entity.AddComponent(new MovementBehavior
            {
                RandomFactor = randomFactor
            });
            entity.AddComponent(new CircleColliderComponent
            {
                Radius = 50
            });

            return entity;
        }

        public Entity CreateMovingRectangleCollider(Scene scene, double x, double y, double randomFactor = 0)
        {
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(x, y),
                Rotation = 0,
                Scale = Vector2.One
            });
            entity.AddComponent(new MovementBehavior
            {
                RandomFactor = randomFactor
            });
            entity.AddComponent(new RectangleColliderComponent()
            {
                Dimension = new Vector2(100, 50)
            });

            return entity;
        }
    }
}