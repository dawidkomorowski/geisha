using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    internal interface IAudioSystem
    {
        void ProcessAudio(Scene scene);
    }

    internal interface IBehaviorSystem
    {
        void ProcessBehaviorFixedUpdate(Scene scene);
        void ProcessBehaviorUpdate(Scene scene, GameTime gameTime);
    }

    internal interface IEntityDestructionSystem
    {
        void DestroyEntities(Scene scene);
    }

    internal interface IInputSystem
    {
        void ProcessInput(Scene scene);
    }

    internal interface IPhysicsSystem
    {
        void ProcessPhysics(Scene scene);
    }

    internal interface IRenderingSystem
    {
        void RenderScene(Scene scene);
    }

    internal interface IEngineSystems
    {
        IAudioSystem AudioSystem { get; }
        IBehaviorSystem BehaviorSystem { get; }
        IEntityDestructionSystem EntityDestructionSystem { get; }
        IInputSystem InputSystem { get; }
        IPhysicsSystem PhysicsSystem { get; }
        IRenderingSystem RenderingSystem { get; }

        string AudioSystemName { get; }
        string BehaviorSystemName { get; }
        string EntityDestructionSystemName { get; }
        string InputSystemName { get; }
        string PhysicsSystemName { get; }
        string RenderingSystemName { get; }
        IReadOnlyCollection<string> SystemsNames { get; }
    }

    internal sealed class EngineSystems : IEngineSystems
    {
        public EngineSystems(
            IAudioSystem audioSystem,
            IBehaviorSystem behaviorSystem,
            IEntityDestructionSystem entityDestructionSystem,
            IInputSystem inputSystem,
            IPhysicsSystem physicsSystem, 
            IRenderingSystem renderingSystem)
        {
            AudioSystem = audioSystem;
            BehaviorSystem = behaviorSystem;
            EntityDestructionSystem = entityDestructionSystem;
            InputSystem = inputSystem;
            PhysicsSystem = physicsSystem;
            RenderingSystem = renderingSystem;

            SystemsNames = new[]
            {
                AudioSystemName,
                BehaviorSystemName,
                EntityDestructionSystemName,
                InputSystemName,
                PhysicsSystemName,
                RenderingSystemName
            };
        }

        public IAudioSystem AudioSystem { get; }
        public IBehaviorSystem BehaviorSystem { get; }
        public IEntityDestructionSystem EntityDestructionSystem { get; }
        public IInputSystem InputSystem { get; }
        public IPhysicsSystem PhysicsSystem { get; }
        public IRenderingSystem RenderingSystem { get; }

        public string AudioSystemName => nameof(AudioSystem);
        public string BehaviorSystemName => nameof(BehaviorSystem);
        public string EntityDestructionSystemName => nameof(EntityDestructionSystem);
        public string InputSystemName => nameof(InputSystem);
        public string PhysicsSystemName => nameof(PhysicsSystem);
        public string RenderingSystemName => nameof(RenderingSystem);
        public IReadOnlyCollection<string> SystemsNames { get; }
    }
}