using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Coroutines;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    internal sealed class CoroutineComponent : BehaviorComponent
    {
        private readonly ICoroutineSystem _coroutineSystem;
        private Transform2DComponent _transform = null!;

        public CoroutineComponent(Entity entity, ICoroutineSystem coroutineSystem) : base(entity)
        {
            _coroutineSystem = coroutineSystem;
        }

        public override void OnStart()
        {
            _transform = Entity.GetComponent<Transform2DComponent>();

            _coroutineSystem.StartCoroutine(MoveToWayPoints(), this);
        }

        private IEnumerator<CoroutineInstruction> MoveToWayPoints()
        {
            while (true)
            {
                yield return Coroutine.Call(MoveTo(0, 0));
                yield return Coroutine.Call(MoveTo(200, -200));
                yield return Coroutine.Call(MoveTo(-300, -100));
            }
        }

        private IEnumerator<CoroutineInstruction> MoveTo(double x, double y)
        {
            var target = new Vector2(x, y);
            var vectorToTarget = target - _transform.Translation;
            var vectorDelta = vectorToTarget.OfLength(2);

            while ((target - _transform.Translation).Length > 3)
            {
                _transform.Translation += vectorDelta;
                yield return Coroutine.Wait(TimeSpan.FromMilliseconds(5));
            }

            _transform.Translation = target;
        }
    }

    internal sealed class CoroutineComponentFactory : ComponentFactory<CoroutineComponent>
    {
        private readonly ICoroutineSystem _coroutineSystem;

        public CoroutineComponentFactory(ICoroutineSystem coroutineSystem)
        {
            _coroutineSystem = coroutineSystem;
        }

        protected override CoroutineComponent CreateComponent(Entity entity) => new(entity, _coroutineSystem);
    }
}