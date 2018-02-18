using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    [Export(typeof(IFixedTimeStepSystem))]
    public class EntityDestructionSystem : IFixedTimeStepSystem
    {
        public int Priority { get; set; } = 100;

        public void FixedUpdate(Scene scene)
        {
            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.IsScheduledForDestruction) scene.RemoveEntity(entity);
            }
        }
    }
}