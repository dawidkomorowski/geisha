﻿using System;
using System.Diagnostics;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;

namespace Sandbox.Behaviors
{
    public class RotateComponent : BehaviorComponent
    {
        public double Velocity { get; set; } = Math.PI / 2;

        public override void OnFixedUpdate()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var transform = Entity.GetComponent<Transform2DComponent>();
            transform.Rotation += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
        }
    }
}