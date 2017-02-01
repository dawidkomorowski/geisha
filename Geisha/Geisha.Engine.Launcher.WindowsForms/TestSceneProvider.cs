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

        [ImportingConstructor]
        public TestSceneProvider(IRenderer2D renderer2D)
        {
            _renderer2D = renderer2D;
        }

        public Scene GetTestScene()
        {
            var scene = new Scene {RootEntity = new Entity()};

            var random = new Random();

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    var dot = new Entity {Parent = scene.RootEntity};
                    dot.AddComponent(new Transform
                    {
                        Translation = new Vector3(i * 100 + random.Next(25), j * 100 + random.Next(25), 0)
                    });
                    dot.AddComponent(new SpriteRenderer {Sprite = CreateDotSprite()});
                }
            }

            var box = new Entity {Parent = scene.RootEntity};
            box.AddComponent(new Transform
            {
                Translation = new Vector3(500, 300, 0)
            });
            box.AddComponent(new SpriteRenderer {Sprite = CreateBoxSprite()});

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

        private ITexture LoadTexture(string filePath)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                return _renderer2D.CreateTexture(stream);
            }
        }
    }
}