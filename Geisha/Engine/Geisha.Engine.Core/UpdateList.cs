using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core
{
    public class UpdateList : IUpdatable
    {
        public IReadOnlyList<IUpdatable> Updatables { get; }

        public UpdateList(IEnumerable<IUpdatable> updatables)
        {
            Updatables = updatables.ToList();
        }

        public void Update(double deltaTime)
        {
            foreach (var updatable in Updatables)
            {
                updatable.Update(deltaTime);
            }
        }

        public void FixedUpdate()
        {
            foreach (var updatable in Updatables)
            {
                updatable.FixedUpdate();
            }
        }
    }
}