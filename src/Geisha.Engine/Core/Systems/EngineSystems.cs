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

    internal interface IEngineSystems
    {
        IAudioSystem AudioSystem { get; }
        IEntityDestructionSystem EntityDestructionSystem { get; }

        string AudioSystemName { get; }
        string EntityDestructionSystemName { get; }
        IReadOnlyCollection<string> SystemsNames { get; }
    }

    internal sealed class EngineSystems : IEngineSystems
    {
        public EngineSystems(IAudioSystem audioSystem, IEntityDestructionSystem entityDestructionSystem)
        {
            AudioSystem = audioSystem;
            EntityDestructionSystem = entityDestructionSystem;

            SystemsNames = new[]
            {
                AudioSystemName,
                EntityDestructionSystemName
            };
        }

        public IAudioSystem AudioSystem { get; }
        public IEntityDestructionSystem EntityDestructionSystem { get; }

        public string AudioSystemName => nameof(AudioSystem);
        public string EntityDestructionSystemName => nameof(EntityDestructionSystem);
        public IReadOnlyCollection<string> SystemsNames { get; }
    }
}