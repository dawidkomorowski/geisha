using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    // TODO It seems inefficient. If final destruction logic would be part of Scene itself then it could be optimized.
    // TODO Lists for entities scheduled for destruction could be internally maintained.
    internal class EntityDestructionSystem : IEntityDestructionSystem
    {
        public void DestroyEntitiesAfterFixedTimeStep(Scene scene)
        {
            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.DestructionTime == DestructionTime.AfterFixedTimeStep) scene.RemoveEntity(entity);
            }
        }

        public void DestroyEntitiesAfterFullFrame(Scene scene)
        {
            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.DestructionTime == DestructionTime.AfterFullFrame) scene.RemoveEntity(entity);
            }
        }
    }
}