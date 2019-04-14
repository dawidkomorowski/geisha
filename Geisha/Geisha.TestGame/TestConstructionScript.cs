﻿using System;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Audio;
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
            if (!IsLevelLoadedFromSave(scene))
            {
                SetUpNewLevel(scene);
            }
        }

        private bool IsLevelLoadedFromSave(Scene scene)
        {
            return scene.AllEntities.Any(e => e.HasComponent<BoxMovementComponent>());
        }

        private void SetUpNewLevel(Scene scene)
        {
            var random = new Random();

            for (var i = 0; i < 150; i++)
            {
                CreateDot(scene, -500 + random.Next(1000), -350 + random.Next(700));
            }

            CreateBox(scene);
            CreateCompass(scene);
            CreateText(scene);
            CreateKeyText(scene);
            CreateCamera(scene);
            CreateBackgroundMusic(scene);
        }

        private void CreateSimpleDot(Scene scene, double x, double y)
        {
            var random = new Random();
            var dot = new Entity();
            dot.AddComponent(new TransformComponent
            {
                Scale = Vector3.One
            });
            dot.AddComponent(new SpriteRendererComponent
                {Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.DotSprite), Visible = true});
            dot.AddComponent(new FollowEllipseComponent
            {
                Velocity = random.NextDouble() * 2 + 1,
                Width = 10,
                Height = 10,
                X = x,
                Y = y
            });

            scene.AddEntity(dot);
        }

        private void CreateDot(Scene scene, double x, double y)
        {
            var random = new Random();
            var dot = new Entity();
            dot.AddComponent(new TransformComponent
            {
                Scale = Vector3.One
            });
            dot.AddComponent(new SpriteRendererComponent {Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.DotSprite)});
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

        private void CreateBox(Scene scene)
        {
            var box = new Entity();
            box.AddComponent(new TransformComponent
            {
                Translation = new Vector3(300, -200, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(0.5, 0.5, 1)
            });
            box.AddComponent(new SpriteRendererComponent
            {
                Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.BoxSprite),
                SortingLayerName = "Box"
            });
            box.AddComponent(new InputComponent {InputMapping = _assetStore.GetAsset<InputMapping>(AssetsIds.PlayerInput)});
            box.AddComponent(new BoxMovementComponent());
            box.AddComponent(new RectangleColliderComponent {Dimension = new Vector2(512, 512)});
            box.AddComponent(new CloseGameOnEscapeKeyComponent());

            var boxLabel = new Entity();
            boxLabel.AddComponent(TransformComponent.Default);
            boxLabel.AddComponent(new TextRendererComponent
            {
                Text = "I am Box!",
                SortingLayerName = "Box",
                Color = Color.FromArgb(255, 255, 0, 0),
                FontSize = FontSize.FromPoints(24)
            });
            box.AddChild(boxLabel);

            scene.AddEntity(box);
        }

        private void CreateCompass(Scene scene)
        {
            var compass = new Entity();
            compass.AddComponent(new TransformComponent
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(0.5, 0.5, 1)
            });
            compass.AddComponent(new SpriteRendererComponent {Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.CompassSprite)});
            compass.AddComponent(new RotateComponent());
            compass.AddComponent(new FollowEllipseComponent {Velocity = 2, Width = 100, Height = 100});

            scene.AddEntity(compass);
        }

        private void CreateText(Scene scene)
        {
            var text = new Entity();
            text.AddComponent(new TransformComponent
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(1, 1, 1)
            });
            text.AddComponent(new TextRendererComponent {Text = "I am Text!", Color = Color.FromArgb(255, 0, 255, 0), FontSize = FontSize.FromPoints(16)});
            text.AddComponent(new FollowEllipseComponent {Velocity = 1, Width = 300, Height = 300});
            text.AddComponent(new RotateComponent());
            text.AddComponent(new DoMagicWithTextComponent());

            scene.AddEntity(text);
        }

        private void CreateKeyText(Scene scene)
        {
            var text = new Entity();
            text.AddComponent(new TransformComponent
            {
                Translation = Vector3.Zero,
                Rotation = Vector3.Zero,
                Scale = Vector3.One
            });
            text.AddComponent(
                new TextRendererComponent {Text = "No key pressed.", Color = Color.FromArgb(255, 255, 0, 255), FontSize = FontSize.FromPoints(40)});
            text.AddComponent(new InputComponent());
            text.AddComponent(new SetTextForCurrentKeyComponent());

            scene.AddEntity(text);
        }

        private void CreateCamera(Scene scene)
        {
            const double resolutionScale = 720d / 720d;

            var camera = new Entity();
            camera.AddComponent(new TransformComponent
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(resolutionScale, resolutionScale, 1)
            });
            camera.AddComponent(new CameraComponent());
            camera.AddComponent(new TopDownCameraForBoxComponent());

            scene.AddEntity(camera);
        }

        private void CreateBackgroundMusic(Scene scene)
        {
            var music = new Entity();
            music.AddComponent(new AudioSourceComponent {Sound = _assetStore.GetAsset<ISound>(AssetsIds.MusicSound)});
            scene.AddEntity(music);
        }
    }
}