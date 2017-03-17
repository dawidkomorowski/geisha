using System;
using System.ComponentModel.Composition;
using System.IO;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Input;
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
                    dot.AddComponent(new FollowEllipseBehavior
                    {
                        Velocity = random.NextDouble() + 1,
                        Width = 10,
                        Height = 10,
                        X = i * 100 + random.Next(25),
                        Y = j * 100 + random.Next(25)
                    });
                }
            }

            for (var i = 0; i < 10; i++)
            {
                var dot = new Entity {Parent = scene.RootEntity};
                dot.AddComponent(new Transform
                {
                    Scale = Vector3.One
                });
                dot.AddComponent(new SpriteRenderer {Sprite = CreateDotSprite()});
                dot.AddComponent(new FollowEllipseBehavior
                {
                    Velocity = random.NextDouble() + 1,
                    Width = 10,
                    Height = 10,
                    X = -500 + random.Next(1000),
                    Y = -350 + random.Next(700)
                });
            }

            var box = new Entity {Parent = scene.RootEntity};
            box.AddComponent(new Transform
            {
                Translation = new Vector3(300, -200, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(0.5, 0.5, 0.5)
            });
            box.AddComponent(new SpriteRenderer {Sprite = CreateBoxSprite()});
            box.AddComponent(new InputComponent {InputMapping = CreateBoxInputMapping()});
            box.AddComponent(new BoxMovement());

            var compass = new Entity {Parent = scene.RootEntity};
            compass.AddComponent(new Transform
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0.5),
                Scale = new Vector3(0.5, 0.5, 0.5)
            });
            compass.AddComponent(new SpriteRenderer {Sprite = CreateCompassSprite()});
            compass.AddComponent(new RotateBehavior());
            compass.AddComponent(new FollowEllipseBehavior {Velocity = 2, Width = 100, Height = 100});

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

        private class RotateBehavior : Behavior
        {
            public double Velocity { get; set; } = Math.PI / 110;

            public override void OnFixedUpdate()
            {
                var transform = Entity.GetComponent<Transform>();
                transform.Rotation += new Vector3(0, 0, Velocity);
            }
        }

        private class FollowEllipseBehavior : Behavior
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

        private class BoxMovement : Behavior
        {
            private bool _wasSetUp = false;

            public double Velocity { get; set; } = 5;

            public override void OnUpdate(double deltaTime)
            {
                var transform = Entity.GetComponent<Transform>();
                var input = Entity.GetComponent<InputComponent>();

                if (!_wasSetUp)
                {
                    //input.BindAxis("MoveUp", value =>
                    //{
                    //    var movementVector = new Vector3(0, value, 0).Unit * Velocity;
                    //    transform.Translation = transform.Translation + movementVector;
                    //});
                    //input.BindAxis("MoveRight", value =>
                    //{
                    //    var movementVector = new Vector3(value, 0, 0).Unit * Velocity;
                    //    transform.Translation = transform.Translation + movementVector;
                    //});
                    input.BindAction("JetRotateRight", () => { transform.Rotation += new Vector3(0, 0, -Math.PI / 8); });

                    _wasSetUp = true;
                }

                // TODO World queries
                // TODO Configuration
                // TODO Common utils for interpolation?
                // TODO Camera component?
                // TODO Visibility (renderer property visible)
                // TODO Enabled (entity or component property?)
                // TODO DeltaTime Smoothing
            }

            public override void OnFixedUpdate()
            {
                var transform = Entity.GetComponent<Transform>();
                var input = Entity.GetComponent<InputComponent>();

                var movementVector = new Vector3(input.GetAxisState("MoveRight"), input.GetAxisState("MoveUp"), 0).Unit;
                transform.Translation = transform.Translation + movementVector * Velocity;
            }
        }

        private InputMapping CreateBoxInputMapping()
        {
            var inputMapping = new InputMapping();

            // Action mappings
            var jetRotateRight = new ActionMappingGroup {ActionName = "JetRotateRight"};
            jetRotateRight.ActionMappings.Add(new ActionMapping
            {
                HardwareInputVariant = new HardwareInputVariant {Key = Key.Space}
            });
            inputMapping.ActionMappingGroups.Add(jetRotateRight);

            // Axis mappings
            var moveUp = new AxisMappingGroup {AxisName = "MoveUp"};
            moveUp.AxisMappings.Add(new AxisMapping
            {
                HardwareInputVariant = new HardwareInputVariant {Key = Key.Up},
                Scale = 1
            });
            moveUp.AxisMappings.Add(new AxisMapping
            {
                HardwareInputVariant = new HardwareInputVariant {Key = Key.Down},
                Scale = -1
            });
            inputMapping.AxisMappingGroups.Add(moveUp);

            var moveRight = new AxisMappingGroup {AxisName = "MoveRight"};
            moveRight.AxisMappings.Add(new AxisMapping
            {
                HardwareInputVariant = new HardwareInputVariant {Key = Key.Right},
                Scale = 1
            });
            moveRight.AxisMappings.Add(new AxisMapping
            {
                HardwareInputVariant = new HardwareInputVariant {Key = Key.Left},
                Scale = -1
            });
            inputMapping.AxisMappingGroups.Add(moveRight);

            return inputMapping;
        }
    }
}