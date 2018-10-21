﻿using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
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