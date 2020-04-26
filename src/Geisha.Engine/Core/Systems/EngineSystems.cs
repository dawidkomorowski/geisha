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

    internal interface IRenderingSystem
    {
        void RenderScene(Scene scene);
    }

    internal interface IEngineSystems
    {
        IAudioSystem AudioSystem { get; }
        IEntityDestructionSystem EntityDestructionSystem { get; }
        IRenderingSystem RenderingSystem { get; }

        string AudioSystemName { get; }
        string EntityDestructionSystemName { get; }
        string RenderingSystemName { get; }
        IReadOnlyCollection<string> SystemsNames { get; }
    }

    internal sealed class EngineSystems : IEngineSystems
    {
        public EngineSystems(
            IAudioSystem audioSystem,
            IEntityDestructionSystem entityDestructionSystem,
            IRenderingSystem renderingSystem)
        {
            AudioSystem = audioSystem;
            EntityDestructionSystem = entityDestructionSystem;
            RenderingSystem = renderingSystem;

            SystemsNames = new[]
            {
                AudioSystemName,
                EntityDestructionSystemName,
                RenderingSystemName
            };
        }

        public IAudioSystem AudioSystem { get; }
        public IEntityDestructionSystem EntityDestructionSystem { get; }
        public IRenderingSystem RenderingSystem { get; }

        public string AudioSystemName => nameof(AudioSystem);
        public string EntityDestructionSystemName => nameof(EntityDestructionSystem);
        public string RenderingSystemName => nameof(RenderingSystem);
        public IReadOnlyCollection<string> SystemsNames { get; }
    }
}