using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    internal interface IEntityDestructionSystem
    {
        void DestroyEntities(Scene scene);
    }

    internal interface IEngineSystems
    {
        IEntityDestructionSystem EntityDestructionSystem { get; }

        string EntityDestructionSystemName { get; }
        IReadOnlyCollection<string> SystemsNames { get; }
    }

    internal sealed class EngineSystems : IEngineSystems
    {
        public EngineSystems(IEntityDestructionSystem entityDestructionSystem)
        {
            EntityDestructionSystem = entityDestructionSystem;

            SystemsNames = new[] {EntityDestructionSystemName};
        }

        public IEntityDestructionSystem EntityDestructionSystem { get; }
        public string EntityDestructionSystemName => nameof(EntityDestructionSystem);
        public IReadOnlyCollection<string> SystemsNames { get; }
    }
}