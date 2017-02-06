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
                        Translation = new Vector3(i * 100 + random.Next(25), j * 100 + random.Next(25), 0),
                        Scale = Vector3.One
                    });
                    dot.AddComponent(new SpriteRenderer {Sprite = CreateDotSprite()});
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
    }
}