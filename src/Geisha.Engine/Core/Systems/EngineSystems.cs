using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    internal interface IAudioSystem
    {
        void ProcessAudio(Scene scene);
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
        IEntityDestructionSystem EntityDestructionSystem { get; }
        IInputSystem InputSystem { get; }
        IPhysicsSystem PhysicsSystem { get; }
        IRenderingSystem RenderingSystem { get; }

        string AudioSystemName { get; }
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
            IEntityDestructionSystem entityDestructionSystem,
            IInputSystem inputSystem,
            IPhysicsSystem physicsSystem,
            IRenderingSystem renderingSystem)
        {
            AudioSystem = audioSystem;
            EntityDestructionSystem = entityDestructionSystem;
            InputSystem = inputSystem;
            RenderingSystem = renderingSystem;
            PhysicsSystem = physicsSystem;

            SystemsNames = new[]
            {
                AudioSystemName,
                EntityDestructionSystemName,
                InputSystemName,
                PhysicsSystemName,
                RenderingSystemName
            };
        }

        public IAudioSystem AudioSystem { get; }
        public IEntityDestructionSystem EntityDestructionSystem { get; }
        public IInputSystem InputSystem { get; }
        public IPhysicsSystem PhysicsSystem { get; }
        public IRenderingSystem RenderingSystem { get; }

        public string AudioSystemName => nameof(AudioSystem);
        public string EntityDestructionSystemName => nameof(EntityDestructionSystem);
        public string InputSystemName => nameof(InputSystem);
        public string PhysicsSystemName => nameof(PhysicsSystem);
        public string RenderingSystemName => nameof(RenderingSystem);
        public IReadOnlyCollection<string> SystemsNames { get; }
    }
}