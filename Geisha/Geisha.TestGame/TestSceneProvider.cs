using System;
using System.ComponentModel.Composition;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.TestGame.Behaviors;

namespace Geisha.TestGame
{
    [Export(typeof(ITestSceneProvider))]
    public class TestSceneProvider : ITestSceneProvider
    {
        private readonly IAssetsLoader _assetsLoader;

        [ImportingConstructor]
        public TestSceneProvider(IAssetsLoader assetsLoader)
        {
            _assetsLoader = assetsLoader;
        }

        public Scene GetTestScene()
        {
            var scene = new Scene {RootEntity = new Entity()};
            var random = new Random();


            for (var i = -5; i < 5; i++)
            {
                for (var j = -2; j < 3; j++)
                {
                    CreateDot(scene, i * 100 + random.Next(25), j * 100 + random.Next(25));
                }
            }

            for (var i = 0; i < 10; i++)
            {
                CreateDot(scene, -500 + random.Next(1000), -350 + random.Next(700));
            }

            CreateBox(scene);
            CreateCompass(scene);

            return scene;
        }

        private Entity CreateDot(Scene scene, double x, double y)
        {
            var random = new Random();
            var dot = new Entity {Parent = scene.RootEntity};
            dot.AddComponent(new Transform
            {
                Scale = Vector3.One
            });
            dot.AddComponent(new SpriteRenderer {Sprite = _assetsLoader.CreateDotSprite()});
            dot.AddComponent(new FollowEllipse
            {
                Velocity = random.NextDouble() * 2 + 1,
                Width = 10,
                Height = 10,
                X = x,
                Y = y
            });
            return dot;
        }

        private Entity CreateBox(Scene scene)
        {
            var box = new Entity {Parent = scene.RootEntity};
            box.AddComponent(new Transform
            {
                Translation = new Vector3(300, -200, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(0.5, 0.5, 0.5)
            });
            box.AddComponent(new SpriteRenderer {Sprite = _assetsLoader.CreateBoxSprite(), SortingLayerName = "Box"});
            box.AddComponent(new InputComponent {InputMapping = InputMappingDefinition.BoxInputMapping});
            box.AddComponent(new BoxMovement());
            return box;
        }

        private Entity CreateCompass(Scene scene)
        {
            var compass = new Entity {Parent = scene.RootEntity};
            compass.AddComponent(new Transform
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0.5),
                Scale = new Vector3(0.5, 0.5, 0.5)
            });
            compass.AddComponent(new SpriteRenderer {Sprite = _assetsLoader.CreateCompassSprite()});
            compass.AddComponent(new Rotate());
            compass.AddComponent(new FollowEllipse {Velocity = 2, Width = 100, Height = 100});
            return compass;
        }
    }
}