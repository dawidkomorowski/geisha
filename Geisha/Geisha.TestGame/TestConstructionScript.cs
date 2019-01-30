using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Rendering;
using Geisha.TestGame.Behaviors;

namespace Geisha.TestGame
{
    public class TestConstructionScript : ISceneConstructionScript
    {
        private readonly IAssetStore _assetStore;

        public TestConstructionScript(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        public string Name => nameof(TestConstructionScript);

        public void Execute(Scene scene)
        {
            var random = new Random();

            for (var i = 0; i < 100; i++)
            {
                CreateDot(scene, -500 + random.Next(1000), -350 + random.Next(700));
            }
        }

        private void CreateDot(Scene scene, double x, double y)
        {
            var random = new Random();
            var dot = new Entity();
            dot.AddComponent(new TransformComponent
            {
                Scale = Vector3.One
            });
            dot.AddComponent(new SpriteRendererComponent {Sprite = _assetStore.GetAsset<Sprite>(new Guid("308012DD-0417-445F-B981-7C1E1C824400"))});
            dot.AddComponent(new FollowEllipseComponent
            {
                Velocity = random.NextDouble() * 2 + 1,
                Width = 10,
                Height = 10,
                X = x,
                Y = y
            });
            dot.AddComponent(new DieFromBoxComponent());
            dot.AddComponent(new CircleColliderComponent {Radius = 32});

            scene.AddEntity(dot);
        }
    }
}