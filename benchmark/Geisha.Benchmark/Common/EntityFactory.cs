using System;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Common
{
    internal interface IEntityFactory
    {
        Entity CreateCamera(Scene scene);
        Entity CreateStaticEllipse(Scene scene, double x, double y, Random random);
        Entity CreateStaticRectangle(Scene scene, double x, double y, Random random);
        Entity CreateMovingEllipse(Scene scene, double x, double y, Random random);
        Entity CreateMovingRectangle(Scene scene, double x, double y, Random random);
        Entity CreateStaticSprite(Scene scene, double x, double y);
        Entity CreateStaticSprite(Scene scene, double x, double y, AssetId spriteAssetId, int orderInLayer = 0);
        Entity CreateMovingSprite(Scene scene, double x, double y, Random random);
        Entity CreateAnimatedSprite(Scene scene, double x, double y, Random random);
        Entity CreateStaticText(Scene scene, double x, double y, Random random);
        Entity CreateMovingText(Scene scene, double x, double y, Random random, bool fixedRotation);
        Entity CreateChangingText(Scene scene, double x, double y, Random random);
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

        public Entity CreateStaticEllipse(Scene scene, double x, double y, Random random)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            var ellipseRenderer = entity.CreateComponent<EllipseRendererComponent>();
            ellipseRenderer.Color = GetRandomColor(random);
            ellipseRenderer.FillInterior = true;
            ellipseRenderer.RadiusX = random.Next(5, 100);
            ellipseRenderer.RadiusY = random.Next(5, 100);

            return entity;
        }

        public Entity CreateStaticRectangle(Scene scene, double x, double y, Random random)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            var rectangleRenderer = entity.CreateComponent<RectangleRendererComponent>();
            rectangleRenderer.Color = GetRandomColor(random);
            rectangleRenderer.FillInterior = true;
            rectangleRenderer.Dimensions = new Vector2(random.Next(5, 100), random.Next(5, 100));

            return entity;
        }

        public Entity CreateMovingEllipse(Scene scene, double x, double y, Random random)
        {
            var entity = CreateStaticEllipse(scene, x, y, random);
            AddMovementBehavior(entity, random);
            return entity;
        }

        public Entity CreateMovingRectangle(Scene scene, double x, double y, Random random)
        {
            var entity = CreateStaticRectangle(scene, x, y, random);
            AddMovementBehavior(entity, random);
            return entity;
        }

        public Entity CreateStaticSprite(Scene scene, double x, double y)
        {
            return CreateStaticSprite(scene, x, y, AssetsIds.PaintColorPalette);
        }

        public Entity CreateStaticSprite(Scene scene, double x, double y, AssetId spriteAssetId, int orderInLayer = 0)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            var spriteRenderer = entity.CreateComponent<SpriteRendererComponent>();
            spriteRenderer.Sprite = _assetStore.GetAsset<Sprite>(spriteAssetId);
            spriteRenderer.OrderInLayer = orderInLayer;

            return entity;
        }

        public Entity CreateMovingSprite(Scene scene, double x, double y, Random random)
        {
            var entity = CreateStaticSprite(scene, x, y);
            AddMovementBehavior(entity, random);
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

        public Entity CreateStaticText(Scene scene, double x, double y, Random random)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            var textRendererComponent = entity.CreateComponent<TextRendererComponent>();
            textRendererComponent.Color = GetRandomColor(random);
            textRendererComponent.Text = GetRandomText(random);
            textRendererComponent.FontSize = FontSize.FromDips(random.Next(10, 25));
            textRendererComponent.MaxWidth = 300;

            return entity;
        }

        public Entity CreateMovingText(Scene scene, double x, double y, Random random, bool fixedRotation)
        {
            var entity = CreateStaticText(scene, x, y, random);
            var movementBehaviorComponent = AddMovementBehavior(entity, random);
            movementBehaviorComponent.FixedRotation = fixedRotation;
            return entity;
        }

        public Entity CreateChangingText(Scene scene, double x, double y, Random random)
        {
            var entity = CreateStaticText(scene, x, y, random);
            entity.CreateComponent<ChangingTextComponent>();
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
            rectangleColliderComponent.Dimensions = new Vector2(100, 50);

            return entity;
        }

        public Entity CreateTurret(Scene scene, double x, double y, Random random)
        {
            var entity = scene.CreateEntity();

            var transform = entity.CreateComponent<Transform2DComponent>();
            transform.Translation = new Vector2(x, y);

            var rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();
            rectangleRendererComponent.Color = GetRandomColor(random);
            rectangleRendererComponent.Dimensions = new Vector2(30, 30);
            rectangleRendererComponent.FillInterior = true;

            CreateCannon(entity, random);

            return entity;
        }

        private static void CreateCannon(Entity entity, Random random)
        {
            var cannonRotor = entity.CreateChildEntity();

            var canonRotorTransform = cannonRotor.CreateComponent<Transform2DComponent>();
            canonRotorTransform.Rotation = random.NextDouble();

            var cannon = cannonRotor.CreateChildEntity();

            var cannonTransform = cannon.CreateComponent<Transform2DComponent>();
            cannonTransform.Translation = new Vector2(0, 10);

            var rectangleRenderer = cannon.CreateComponent<RectangleRendererComponent>();
            rectangleRenderer.Color = GetRandomColor(random);
            rectangleRenderer.Dimensions = new Vector2(10, 30);
            rectangleRenderer.FillInterior = true;
            rectangleRenderer.OrderInLayer = 1;

            var cannonBehavior = cannon.CreateComponent<CannonBehaviorComponent>();
            cannonBehavior.Random = random;
        }

        private static MovementBehaviorComponent AddMovementBehavior(Entity entity, Random random)
        {
            var movementBehavior = entity.CreateComponent<MovementBehaviorComponent>();
            movementBehavior.RandomFactor = random.NextDouble() * 10;
            return movementBehavior;
        }

        private static Color GetRandomColor(Random random) => Color.FromArgb(1, random.NextDouble(), random.NextDouble(), random.NextDouble());

        private static string GetRandomText(Random random)
        {
            var texts = new string[5];
            texts[0] =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc tristique scelerisque nunc non faucibus. Ut porttitor ante egestas, viverra odio eget, pharetra leo. Mauris id interdum metus, eu semper leo. Pellentesque vel blandit metus. Curabitur facilisis purus risus, sit amet faucibus tortor aliquam sit amet. Vestibulum sed pulvinar ex. Proin blandit efficitur sem non viverra. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nam eget lacus id lorem laoreet vehicula.";
            texts[1] =
                "Proin sagittis quam elementum elementum lacinia. Curabitur at tristique nibh. Proin sit amet tincidunt elit. Aenean porta ex ut sapien sodales pulvinar. Nunc magna quam, sollicitudin eu orci rhoncus, bibendum lacinia magna. Praesent vehicula ut purus sed condimentum. Suspendisse id mi vitae ex aliquam dignissim vitae nec diam. Nulla mattis magna nec felis facilisis pellentesque. Donec velit neque, lobortis vitae eros ac, rutrum gravida augue.";
            texts[2] =
                "Nunc elementum ex eu ex aliquam vehicula. Vivamus luctus nisi tortor, nec tristique ligula vehicula nec. Nam nec leo eu sapien condimentum vestibulum eu eu massa. Nullam sed eleifend libero, sit amet pulvinar risus. Etiam pretium justo enim. Praesent posuere sapien nec velit volutpat porttitor. Donec erat elit, condimentum sit amet augue non, mattis iaculis velit. Fusce sollicitudin eget ante a ultricies. Cras blandit nulla a lectus ultricies iaculis. Nulla facilisi. Nulla eu facilisis leo. Vestibulum lobortis venenatis sapien sed consectetur. Praesent tempus velit fermentum, mollis dui nec, porttitor risus. Aenean aliquam eros at cursus vulputate. Sed id interdum arcu.";
            texts[3] =
                "Nunc eget lorem quis turpis suscipit hendrerit id sagittis nibh. Maecenas quis condimentum est, at euismod est. Sed interdum ac erat sit amet gravida. Aliquam consequat eu sapien quis commodo. Donec sed tempus lacus. Interdum et malesuada fames ac ante ipsum primis in faucibus. Pellentesque fermentum lacus in fermentum commodo. In in lectus eu risus scelerisque pulvinar. Suspendisse rutrum, nisi a viverra sollicitudin, sapien ante sodales neque, eu feugiat eros sem nec nunc. Interdum et malesuada fames ac ante ipsum primis in faucibus. Interdum et malesuada fames ac ante ipsum primis in faucibus. Donec congue tincidunt augue in tempor. Vivamus sed consequat ex. Ut consectetur laoreet mauris eu congue. Nunc dictum in nibh et venenatis.";
            texts[4] =
                "Praesent mattis, elit quis commodo cursus, magna ipsum tincidunt est, vitae porttitor est neque sed orci. Duis pretium bibendum justo, bibendum pulvinar augue imperdiet vitae. Ut rutrum leo eget urna gravida dictum. Praesent sit amet interdum ipsum. In sed enim condimentum, tincidunt enim a, tristique justo. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed metus tortor, consequat pellentesque pulvinar et, placerat a metus. Mauris non orci et est facilisis pretium. Pellentesque lacinia tellus nec mattis varius.";

            return texts[random.Next(5)];
        }
    }
}