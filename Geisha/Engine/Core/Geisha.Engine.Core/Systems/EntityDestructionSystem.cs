using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    [Export(typeof(IVariableTimeStepSystem))]
    public class EntityDestructionSystem : IVariableTimeStepSystem
    {
        public int Priority { get; set; } = 3;

        public void Update(Scene scene, double deltaTime)
        {
            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.IsScheduledForDestruction) scene.RemoveEntity(entity);
            }
        }
    }
}