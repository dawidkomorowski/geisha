using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;

namespace Geisha.TestUtils
{
    public static class TestSceneFactory
    {
        public static Scene Create() => Create(Enumerable.Empty<IComponentFactory>());

        public static Scene Create(IEnumerable<IComponentFactory> customComponentFactories)
        {
            var factories = new List<IComponentFactory>();

            // Animation
            factories.Add(new SpriteAnimationComponentFactory());

            // Audio
            factories.Add(new AudioSourceComponentFactory());

            // Core
            factories.Add(new Transform2DComponentFactory());
            factories.Add(new Transform3DComponentFactory());

            // Input
            factories.Add(new InputComponentFactory());

            // Physics
            factories.Add(new CircleColliderComponentFactory());
            factories.Add(new RectangleColliderComponentFactory());
            factories.Add(new TileColliderComponentFactory());
            factories.Add(new KinematicRigidBody2DComponentFactory());

            // Rendering
            factories.Add(new CameraComponentFactory());
            factories.Add(new EllipseRendererComponentFactory());
            factories.Add(new RectangleRendererComponentFactory());
            factories.Add(new SpriteRendererComponentFactory());
            factories.Add(new TextRendererComponentFactory());

            // Custom component factories
            factories.AddRange(customComponentFactories);

            var componentFactoryProvider = new ComponentFactoryProvider();
            componentFactoryProvider.Initialize(factories);

            return new Scene(componentFactoryProvider);
        }
    }
}