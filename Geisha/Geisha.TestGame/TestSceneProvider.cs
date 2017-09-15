using System;
using System.ComponentModel.Composition;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Rendering;
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
            var scene = new Scene();
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
            CreateText(scene);
            CreateCamera(scene);

            return scene;
        }

        private void CreateDot(Scene scene, double x, double y)
        {
            var random = new Random();
            var dot = new Entity();
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
            dot.AddComponent(new DieFromBox());

            scene.AddEntity(dot);
        }

        private void CreateBox(Scene scene)
        {
            var box = new Entity();
            box.AddComponent(new Transform
            {
                Translation = new Vector3(300, -200, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(0.5, 0.5, 1)
            });
            const string sortingLayerName = "Box";
            box.AddComponent(new SpriteRenderer {Sprite = _assetsLoader.CreateBoxSprite(), SortingLayerName = sortingLayerName});
            //box.AddComponent(new TextRenderer {Text = "I am Box!", SortingLayerName = sortingLayerName});
            box.AddComponent(new InputComponent {InputMapping = InputMappingDefinition.BoxInputMapping});
            box.AddComponent(new BoxMovement());

            scene.AddEntity(box);
        }

        private void CreateCompass(Scene scene)
        {
            var compass = new Entity();
            compass.AddComponent(new Transform
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(0.5, 0.5, 1)
            });
            compass.AddComponent(new SpriteRenderer {Sprite = _assetsLoader.CreateCompassSprite()});
            compass.AddComponent(new Rotate());
            compass.AddComponent(new FollowEllipse {Velocity = 2, Width = 100, Height = 100});

            scene.AddEntity(compass);
        }

        private void CreateText(Scene scene)
        {
            var text = new Entity();
            text.AddComponent(new Transform
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(1, 1, 1)
            });
            text.AddComponent(new TextRenderer {Text = "I am Text!", Color = Color.FromArgb(255, 0, 255, 0)});
            text.AddComponent(new FollowEllipse {Velocity = 1, Width = 300, Height = 300});
            text.AddComponent(new Rotate());
            text.AddComponent(new DoMagicWithText());

            scene.AddEntity(text);
        }

        private void CreateCamera(Scene scene)
        {
            var resolutionScale = 720d / 720d;

            var camera = new Entity();
            camera.AddComponent(new Transform
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(resolutionScale, resolutionScale, 1)
            });
            camera.AddComponent(new Camera());
            camera.AddComponent(new TopDownCameraForBox());

            scene.AddEntity(camera);
        }
    }
}