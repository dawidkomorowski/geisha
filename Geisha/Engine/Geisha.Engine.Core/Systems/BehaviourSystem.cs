using System;
using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    [Export(typeof(ISystem))]
    public class BehaviourSystem : ISystem
    {
        public int Priority { get; set; }
        public Scene Scene { get; set; }

        public void Update(double deltaTime)
        {
            PerformUpdate(behaviour => behaviour.OnUpdate(deltaTime));
        }

        public void FixedUpdate()
        {
            PerformUpdate(behaviour => behaviour.OnFixedUpdate());
        }

        private void PerformUpdate(Action<Behaviour> updateAction)
        {
            foreach (var entity in Scene.RootEntity.GetChildrenRecursivelyIncludingRoot().ToList())
            {
                if (entity.HasComponent<Behaviour>())
                {
                    var behaviours = entity.GetComponents<Behaviour>();
                    foreach (var behaviour in behaviours)
                    {
                        behaviour.Entity = entity;
                        updateAction(behaviour);
                    }
                }
            }
        }
    }
}