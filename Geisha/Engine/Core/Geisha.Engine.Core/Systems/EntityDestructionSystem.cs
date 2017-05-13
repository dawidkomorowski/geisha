using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    [Export(typeof(ISystem))]
    public class EntityDestructionSystem : ISystem
    {
        public int Priority { get; set; } = 3;
        public UpdateMode UpdateMode { get; set; } = UpdateMode.Variable;

        public void Update(Scene scene, double deltaTime)
        {
            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.IsScheduledForDestruction)
                {
                    scene.RemoveEntity(entity);
                }
            }
        }

        public void FixedUpdate(Scene scene)
        {
            throw new System.NotImplementedException();
        }
    }
}