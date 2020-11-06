using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
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