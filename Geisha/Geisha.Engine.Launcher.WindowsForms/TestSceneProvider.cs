using System;
using System.ComponentModel.Composition;
using System.IO;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Launcher.WindowsForms
{
    [Export(typeof(ITestSceneProvider))]
    public class TestSceneProvider : ITestSceneProvider
    {
        private readonly IRenderer2D _renderer2D;
        private const string ResourcesRootPath = @"..\..\TestResources\";

        [ImportingConstructor]
        public TestSceneProvider(IRenderer2D renderer2D)
        {
            _renderer2D = renderer2D;
        }

        public Scene GetTestScene()
        {
            var scene = new Scene {RootEntity = new Entity()};

            var random = new Random();

            for (var i = -5; i < 5; i++)
            {
                for (var j = -2; j < 3; j++)
                {
                    var dot = new Entity {Parent = scene.RootEntity};
                    dot.AddComponent(new Transform
                    {
                        //Translation = new Vector3(i * 100 + random.Next(25), j * 100 + random.Next(25), 0),
                        Scale = Vector3.One
                    });
                    dot.AddComponent(new SpriteRenderer {Sprite = CreateDotSprite()});
                    dot.AddComponent(new FollowElipseBehaviour
                    {
                        Velocity = random.NextDouble() + 1,
                        Width = 10,
                        Height = 10,
                        X = i * 100 + random.Next(25),
                        Y = j * 100 + random.Next(25)
                    });
                }
            }

            var box = new Entity {Parent = scene.RootEntity};
            box.AddComponent(new Transform
            {
                Translation = new Vector3(300, -200, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(0.5, 0.5, 0.5)
            });
            box.AddComponent(new SpriteRenderer {Sprite = CreateBoxSprite()});

            var compass = new Entity {Parent = scene.RootEntity};
            compass.AddComponent(new Transform
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0.5),
                Scale = new Vector3(0.5, 0.5, 0.5)
            });
            compass.AddComponent(new SpriteRenderer {Sprite = CreateCompassSprite()});
            compass.AddComponent(new RotateBehaviour {Velocity = 2});
            compass.AddComponent(new FollowElipseBehaviour {Velocity = 2, Width = 100, Height = 100});

            return scene;
        }

        private Sprite CreateDotSprite()
        {
            var dotTexture = LoadTexture("Dot.png");

            return new Sprite
            {
                SourceTexture = dotTexture,
                SourceUV = Vector2.Zero,
                SourceDimension = dotTexture.Dimension,
                SourceAnchor = dotTexture.Dimension / 2,
                PixelsPerUnit = 1
            };
        }

        private Sprite CreateBoxSprite()
        {
            var boxTexture = LoadTexture("box.jpg");

            return new Sprite
            {
                SourceTexture = boxTexture,
                SourceUV = Vector2.Zero,
                SourceDimension = boxTexture.Dimension,
                SourceAnchor = boxTexture.Dimension / 2,
                PixelsPerUnit = 1
            };
        }

        private Sprite CreateCompassSprite()
        {
            var compassTexture = LoadTexture("compass_texture.png");

            return new Sprite
            {
                SourceTexture = compassTexture,
                SourceUV = Vector2.Zero,
                SourceDimension = compassTexture.Dimension,
                SourceAnchor = compassTexture.Dimension / 2,
                PixelsPerUnit = 1
            };
        }

        private ITexture LoadTexture(string filePath)
        {
            filePath = Path.Combine(ResourcesRootPath, filePath);
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                return _renderer2D.CreateTexture(stream);
            }
        }

        private class RotateBehaviour : Behaviour
        {
            public double Velocity { get; set; } = 0.1;

            public override void OnUpdate(double deltaTime)
            {
                var transform = Entity.GetComponent<Transform>();
                transform.Rotation += new Vector3(0, 0, deltaTime * Velocity);
            }
        }

        private class FollowElipseBehaviour : Behaviour
        {
            private double _totalTime;

            public double Velocity { get; set; } = 0.1;
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }

            public override void OnUpdate(double deltaTime)
            {
                var transform = Entity.GetComponent<Transform>();
                transform.Translation = new Vector3(Width * Math.Sin(_totalTime * Velocity) + X,
                    Height * Math.Cos(_totalTime * Velocity) + Y, transform.Translation.Z);

                _totalTime += deltaTime;
            }
        }
    }
}