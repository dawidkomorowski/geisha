using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    // TODO Should it be implemented as IFixedTimeStepSystem or be more directly run by game loop? Maybe all "engine-provided systems" should be more directly called in game loop i.e. IRenderingSystem. It solved custom parameters issue.
    internal class EntityDestructionSystem : IFixedTimeStepSystem
    {
        public string Name => GetType().FullName;

        public void FixedUpdate(Scene scene)
        {
            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.IsScheduledForDestruction) scene.RemoveEntity(entity);
            }
        }
    }
}