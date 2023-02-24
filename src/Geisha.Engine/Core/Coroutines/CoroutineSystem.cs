using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Coroutines
{
    // TODO Better handling of SwitchTo?
    // TODO OnStart() when switching to coroutine?
    // TODO FixedTimeStep coroutine execution
    // TODO Refactor?
    // TODO How should exceptions work in coroutines?
    public interface ICoroutineSystem
    {
        Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine);
        Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Entity owner);
        Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Component owner);
        Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine);
        Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, Entity owner);
        Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, Component owner);
    }

    internal sealed class CoroutineSystem : ICoroutineSystem, ISceneObserver
    {
        private readonly List<Coroutine> _coroutines = new();
        private readonly List<Coroutine> _justStartedCoroutines = new();
        private readonly Dictionary<Entity, List<Coroutine>> _coroutineIndexByEntity = new();
        private readonly Dictionary<Component, List<Coroutine>> _coroutineIndexByComponent = new();

        #region Implementation of ICoroutineSystem

        public Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine)
            => TrackCoroutineOwnership(new Coroutine(coroutine));

        public Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Entity owner)
            => TrackCoroutineOwnership(new Coroutine(coroutine, owner));

        public Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Component owner)
            => TrackCoroutineOwnership(new Coroutine(coroutine, owner));

        public Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine)
            => StartCoroutine(CreateCoroutine(coroutine));

        public Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, Entity owner)
            => StartCoroutine(CreateCoroutine(coroutine, owner));

        public Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, Component owner)
            => StartCoroutine(CreateCoroutine(coroutine, owner));

        #endregion

        public void ProcessCoroutines(GameTime gameTime)
        {
            _coroutines.AddRange(_justStartedCoroutines);
            _justStartedCoroutines.Clear();

            // TODO This is inefficient when number of coroutines is big.
            for (var i = _coroutines.Count - 1; i >= 0; i--)
            {
                var coroutine = _coroutines[i];
                if (coroutine.State is CoroutineState.Completed or CoroutineState.Aborted)
                {
                    RemoveCoroutine(coroutine);
                }
            }

            var switchToCoroutines = new List<(Coroutine target, Coroutine source)>();
            foreach (var coroutine in _coroutines)
            {
                var targetCoroutine = coroutine.Execute(gameTime);
                if (targetCoroutine is not null)
                {
                    switchToCoroutines.Add((targetCoroutine, coroutine));
                }
            }

            foreach (var (target, source) in switchToCoroutines)
            {
                _coroutines.Remove(source);
                _coroutines.Add(target);
                target.OnStart();
            }
        }

        #region Implementation of ISceneObserver

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
            if (_coroutineIndexByEntity.Remove(entity, out var coroutines))
            {
                foreach (var coroutine in coroutines)
                {
                    // TODO Coroutine is aborted twice when it is owned by Component and the Entity is removed.
                    coroutine.Abort();
                }
            }
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
        }

        public void OnComponentRemoved(Component component)
        {
            if (_coroutineIndexByComponent.Remove(component, out var coroutines))
            {
                foreach (var coroutine in coroutines)
                {
                    // TODO Exception here seems still possible for completed coroutine
                    coroutine.Abort();
                }
            }
        }

        #endregion

        private Coroutine TrackCoroutineOwnership(Coroutine coroutine)
        {
            if (coroutine.OwnerEntity is not null)
            {
                if (_coroutineIndexByEntity.TryGetValue(coroutine.OwnerEntity, out var coroutines))
                {
                    coroutines.Add(coroutine);
                }
                else
                {
                    _coroutineIndexByEntity[coroutine.OwnerEntity] = new List<Coroutine> { coroutine };
                }
            }

            if (coroutine.OwnerComponent is not null)
            {
                if (_coroutineIndexByComponent.TryGetValue(coroutine.OwnerComponent, out var coroutines))
                {
                    coroutines.Add(coroutine);
                }
                else
                {
                    _coroutineIndexByComponent[coroutine.OwnerComponent] = new List<Coroutine> { coroutine };
                }
            }

            return coroutine;
        }

        private Coroutine StartCoroutine(Coroutine coroutine)
        {
            _justStartedCoroutines.Add(coroutine);
            coroutine.OnStart();
            return coroutine;
        }

        private void RemoveCoroutine(Coroutine coroutine)
        {
            _coroutines.Remove(coroutine);

            if (coroutine.OwnerEntity is not null)
            {
                var coroutines = _coroutineIndexByEntity[coroutine.OwnerEntity];
                coroutines.Remove(coroutine);

                if (coroutines.Count == 0)
                {
                    _coroutineIndexByEntity.Remove(coroutine.OwnerEntity);
                }
            }

            if (coroutine.OwnerComponent is not null)
            {
                var coroutines = _coroutineIndexByComponent[coroutine.OwnerComponent];
                coroutines.Remove(coroutine);

                if (coroutines.Count == 0)
                {
                    _coroutineIndexByComponent.Remove(coroutine.OwnerComponent);
                }
            }
        }
    }
}