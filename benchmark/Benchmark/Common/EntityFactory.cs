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

        public Entity CreateMovingSprite(Scene scene, double x, double y, Random random)
        {
            var entity = CreateStaticSprite(scene, x, y);
            entity.AddComponent(new MovementBehaviorComponent
            {
                RandomFactor = random.NextDouble() * 10
            });

            return entity;
        }

        public Entity CreateAnimatedSprite(Scene scene, double x, double y, Random random)
        {
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(x, y),
                Rotation = 0,
                Scale = Vector2.One
            });
            entity.AddComponent(new SpriteRendererComponent());

            var spriteAnimationComponent = new SpriteAnimationComponent
            {
                PlayInLoop = true
            };
            entity.AddComponent(spriteAnimationComponent);

            spriteAnimationComponent.AddAnimation("Explosion", _assetStore.GetAsset<SpriteAnimation>(AssetsIds.ExplosionAnimation));
            spriteAnimationComponent.PlayAnimation("Explosion");
            spriteAnimationComponent.Position = random.NextDouble();

            return entity;
        }

        public Entity CreateMovingCircleCollider(Scene scene, double x, double y, Random random)
        {
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(x, y),
                Rotation = 0,
                Scale = Vector2.One
            });
            entity.AddComponent(new MovementBehaviorComponent
            {
                RandomFactor = random.NextDouble()
            });
            entity.AddComponent(new CircleColliderComponent
            {
                Radius = 50
            });

            return entity;
        }

        public Entity CreateMovingRectangleCollider(Scene scene, double x, double y, Random random)
        {
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(x, y),
                Rotation = 0,
                Scale = Vector2.One
            });
            entity.AddComponent(new MovementBehaviorComponent
            {
                RandomFactor = random.NextDouble()
            });
            entity.AddComponent(new RectangleColliderComponent
            {
                Dimension = new Vector2(100, 50)
            });

            return entity;
        }

        public Entity CreateTurret(Scene scene, double x, double y, Random random)
        {
            var entity = new Entity();
            scene.AddEntity(entity);

            entity.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(x, y),
                Rotation = 0,
                Scale = Vector2.One
            });
            entity.AddComponent(new RectangleRendererComponent
            {
                Color = Color.FromArgb(1, random.NextDouble(), random.NextDouble(), random.NextDouble()),
                Dimension = new Vector2(30, 30),
                FillInterior = true
            });

            CreateCannon(entity, random);

            return entity;
        }

        private Entity CreateCannon(Entity entity, Random random)
        {
            var cannonRotor = new Entity();
            entity.AddChild(cannonRotor);
            cannonRotor.AddComponent(new Transform2DComponent
            {
                Translation = Vector2.Zero,
                Rotation = random.NextDouble(),
                Scale = Vector2.One
            });

            var cannon = new Entity();
            cannonRotor.AddChild(cannon);

            cannon.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(0, 10),
                Rotation = 0,
                Scale = Vector2.One
            });
            cannon.AddComponent(new RectangleRendererComponent
            {
                Color = Color.FromArgb(1, random.NextDouble(), random.NextDouble(), random.NextDouble()),
                Dimension = new Vector2(10, 30),
                FillInterior = true,
                OrderInLayer = 1
            });
            cannon.AddComponent(new CannonBehaviorComponent { Random = random });

            return cannon;
        }
    }
}