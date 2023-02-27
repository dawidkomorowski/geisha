using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Coroutines
{
    // TODO Better handling of SwitchTo?
    // TODO OnStart() when switching to coroutine?
    // TODO SwitchTo to already running top level coroutine?
    // TODO SwitchTo Completed/Aborted coroutine?
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
        private readonly HashSet<Coroutine> _coroutinesToRemove = new();
        private readonly List<(Coroutine from, Coroutine to)> _switchToCoroutines = new();
        private readonly Dictionary<Entity, List<Coroutine>> _coroutineIndexByEntity = new();
        private readonly Dictionary<Component, List<Coroutine>> _coroutineIndexByComponent = new();

        #region Implementation of ICoroutineSystem

        public Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine)
            => TrackCoroutineOwnership(new Coroutine(this, coroutine));

        public Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Entity owner)
            => TrackCoroutineOwnership(new Coroutine(this, coroutine, owner));

        public Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Component owner)
            => TrackCoroutineOwnership(new Coroutine(this, coroutine, owner));

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

            foreach (var coroutine in _coroutinesToRemove)
            {
                RemoveCoroutine(coroutine);
            }

            _coroutinesToRemove.Clear();

            foreach (var coroutine in _coroutines)
            {
                coroutine.Execute(gameTime);
            }

            foreach (var (from, to) in _switchToCoroutines)
            {
                _coroutines.Remove(from);
                _coroutines.Add(to);
                to.OnStart();
            }

            _switchToCoroutines.Clear();
        }

        #region Implementation of ISceneObserver

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
            if (_coroutineIndexByEntity.TryGetValue(entity, out var coroutines))
            {
                foreach (var coroutine in coroutines)
                {
                    if (coroutine.State is not CoroutineState.Completed)
                    {
                        coroutine.Abort();
                    }
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
            if (_coroutineIndexByComponent.TryGetValue(component, out var coroutines))
            {
                foreach (var coroutine in coroutines)
                {
                    if (coroutine.State is not CoroutineState.Completed)
                    {
                        coroutine.Abort();
                    }
                }
            }
        }

        #endregion

        #region Internal API for Coroutine class

        /// <summary>
        ///     Internal API for <see cref="Coroutine" /> class.
        /// </summary>
        internal void OnCoroutineAborted(Coroutine coroutine)
        {
            _coroutinesToRemove.Add(coroutine);
        }

        /// <summary>
        ///     Internal API for <see cref="Coroutine" /> class.
        /// </summary>
        internal void OnCoroutineCompleted(Coroutine coroutine)
        {
            _coroutinesToRemove.Add(coroutine);
        }

        internal void OnSwitchToCoroutine(Coroutine from, Coroutine to)
        {
            if (_coroutines.Contains(to))
            {
                throw new InvalidOperationException("Cannot switch to coroutine that is already active.");
            }

            _switchToCoroutines.Add((from, to));
        }

        #endregion

        internal int ActiveCoroutinesCount => _coroutines.Count;

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