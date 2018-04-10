using System;
using System.ComponentModel.Composition;
using System.IO;
using Geisha.Common.Math;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Rendering;
using Geisha.TestGame.Behaviors;

namespace Geisha.TestGame
{
    [Export(typeof(ITestSceneProvider))]
    public class TestSceneProvider : ITestSceneProvider
    {
        private const string AssetsRootPath = @"Assets\";

        private readonly IAssetsLoader _assetsLoader;
        private readonly IAssetStore _assetStore;
        private readonly IEngineManager _engineManager;
        private readonly ISceneLoader _sceneLoader;

        [ImportingConstructor]
        public TestSceneProvider(IAssetsLoader assetsLoader, IEngineManager engineManager, ISceneLoader sceneLoader, IAssetStore assetStore)
        {
            _assetsLoader = assetsLoader;
            _engineManager = engineManager;
            _sceneLoader = sceneLoader;
            _assetStore = assetStore;
        }

        public Scene GetTestScene()
        {
            RegisterGameAssets();

            TestSceneLoader();

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
            CreateKeyText(scene);
            CreateCamera(scene);
            CreateBackgroundMusic(scene);

            return scene;
        }

        private void RegisterGameAssets()
        {
            _assetStore.RegisterAsset(new AssetInfo(typeof(Sprite), new Guid("308012DD-0417-445F-B981-7C1E1C824400"),
                Path.Combine(AssetsRootPath, "dot.sprite")));
            _assetStore.RegisterAsset(new AssetInfo(typeof(Sprite), new Guid("72D0650C-996F-4E61-904C-617E940326DE"),
                Path.Combine(AssetsRootPath, "box.sprite")));
            _assetStore.RegisterAsset(new AssetInfo(typeof(Sprite), new Guid("09400BA1-A7AB-4752-ADC2-C6535898685C"),
                Path.Combine(AssetsRootPath, "compass.sprite")));

            _assetStore.RegisterAsset(new AssetInfo(typeof(InputMapping), new Guid("4D5E957B-6176-4FFA-966D-5C3403909D9A"),
                Path.Combine(AssetsRootPath, "player_key_binding.input")));
        }

        private void CreateDot(Scene scene, double x, double y)
        {
            var random = new Random();
            var dot = new Entity();
            dot.AddComponent(new Transform
            {
                Scale = Vector3.One
            });
            dot.AddComponent(new SpriteRenderer {Sprite = _assetStore.GetAsset<Sprite>(new Guid("308012DD-0417-445F-B981-7C1E1C824400"))});
            dot.AddComponent(new FollowEllipse
            {
                Velocity = random.NextDouble() * 2 + 1,
                Width = 10,
                Height = 10,
                X = x,
                Y = y
            });
            dot.AddComponent(new DieFromBox(_assetsLoader.DotDieSound));
            dot.AddComponent(new CircleCollider {Radius = 32});

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
            box.AddComponent(new SpriteRenderer
            {
                Sprite = _assetStore.GetAsset<Sprite>(new Guid("72D0650C-996F-4E61-904C-617E940326DE")),
                SortingLayerName = "Box"
            });
            box.AddComponent(new InputComponent {InputMapping = _assetStore.GetAsset<InputMapping>(new Guid("4D5E957B-6176-4FFA-966D-5C3403909D9A"))});
            box.AddComponent(new BoxMovement());
            box.AddComponent(new RectangleCollider {Dimension = new Vector2(512, 512)});
            box.AddComponent(new CloseGameOnEscapeKey(_engineManager));

            var boxLabel = new Entity();
            boxLabel.AddComponent(Transform.Default);
            boxLabel.AddComponent(new TextRenderer {Text = "I am Box!", SortingLayerName = "Box", Color = Color.FromArgb(255, 255, 0, 0), FontSize = 24});
            box.AddChild(boxLabel);

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
            compass.AddComponent(new SpriteRenderer {Sprite = _assetStore.GetAsset<Sprite>(new Guid("09400BA1-A7AB-4752-ADC2-C6535898685C"))});
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
            text.AddComponent(new TextRenderer {Text = "I am Text!", Color = Color.FromArgb(255, 0, 255, 0), FontSize = 16});
            text.AddComponent(new FollowEllipse {Velocity = 1, Width = 300, Height = 300});
            text.AddComponent(new Rotate());
            text.AddComponent(new DoMagicWithText());

            scene.AddEntity(text);
        }

        private void CreateKeyText(Scene scene)
        {
            var text = new Entity();
            text.AddComponent(new Transform
            {
                Translation = Vector3.Zero,
                Rotation = Vector3.Zero,
                Scale = Vector3.One
            });
            text.AddComponent(new TextRenderer {Text = "No key pressed.", Color = Color.FromArgb(255, 255, 0, 255), FontSize = 40});
            text.AddComponent(new InputComponent());
            text.AddComponent(new SetTextForCurrentKey());

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

        private void CreateBackgroundMusic(Scene scene)
        {
            var music = new Entity();
            music.AddComponent(new AudioSource {Sound = _assetsLoader.Music});
            scene.AddEntity(music);
        }

        private void TestSceneLoader()
        {
            var scene = new Scene();
            scene.AddEntity(new Entity {Name = "Some entity"});
            scene.AddEntity(new Entity());
            scene.AddEntity(new Entity());
            var root = new Entity();
            root.AddChild(new Entity());
            root.AddChild(new Entity());
            var child = new Entity();
            root.AddChild(child);
            child.AddChild(new Entity());
            child.AddChild(new Entity());
            child.AddChild(new Entity());

            root.AddComponent(Transform.Default);
            root.AddComponent(new Transform
            {
                Translation = new Vector3(1.23, 2.34, 3.45),
                Rotation = new Vector3(4.56, 5.67, 6.78),
                Scale = new Vector3(7.89, 8.90, 9.00)
            });
            root.AddComponent(new SpriteRenderer
            {
                Sprite = _assetStore.GetAsset<Sprite>(new Guid("72D0650C-996F-4E61-904C-617E940326DE")),
                SortingLayerName = "Box"
            });

            scene.AddEntity(root);
            _sceneLoader.Save(scene, "SomeScene.scene");
            scene = _sceneLoader.Load("SomeScene.scene");
        }
    }
}