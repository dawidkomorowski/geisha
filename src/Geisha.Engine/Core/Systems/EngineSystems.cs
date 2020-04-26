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

    internal interface IRenderingSystem
    {
        void RenderScene(Scene scene);
    }

    internal interface IEngineSystems
    {
        IAudioSystem AudioSystem { get; }
        IEntityDestructionSystem EntityDestructionSystem { get; }
        IInputSystem InputSystem { get; }
        IRenderingSystem RenderingSystem { get; }

        string AudioSystemName { get; }
        string EntityDestructionSystemName { get; }
        string InputSystemName { get; }
        string RenderingSystemName { get; }
        IReadOnlyCollection<string> SystemsNames { get; }
    }

    internal sealed class EngineSystems : IEngineSystems
    {
        public EngineSystems(
            IAudioSystem audioSystem,
            IEntityDestructionSystem entityDestructionSystem,
            IInputSystem inputSystem,
            IRenderingSystem renderingSystem)
        {
            AudioSystem = audioSystem;
            EntityDestructionSystem = entityDestructionSystem;
            InputSystem = inputSystem;
            RenderingSystem = renderingSystem;

            SystemsNames = new[]
            {
                AudioSystemName,
                EntityDestructionSystemName,
                InputSystemName,
                RenderingSystemName
            };
        }

        public IAudioSystem AudioSystem { get; }
        public IEntityDestructionSystem EntityDestructionSystem { get; }
        public IInputSystem InputSystem { get; }
        public IRenderingSystem RenderingSystem { get; }

        public string AudioSystemName => nameof(AudioSystem);
        public string EntityDestructionSystemName => nameof(EntityDestructionSystem);
        public string InputSystemName => nameof(InputSystem);
        public string RenderingSystemName => nameof(RenderingSystem);
        public IReadOnlyCollection<string> SystemsNames { get; }
    }
}