using System;
using System.Collections.Generic;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Coroutines
{
    public interface ICoroutineSystem
    {
        Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep);

        Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Entity owner,
            CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep);

        Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Component owner,
            CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep);

        Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep);

        Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, Entity owner,
            CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep);

        Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, Component owner,
            CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep);
    }

    internal sealed class CoroutineSystem : ICoroutineSystem, ISceneObserver, ICoroutineGameLoopStep
    {
        private readonly Context _fixedTimeStepContext = Context.Create();
        private readonly Context _variableTimeStepContext = Context.Create();
        private readonly Dictionary<Entity, List<Coroutine>> _coroutineIndexByEntity = new();
        private readonly Dictionary<Component, List<Coroutine>> _coroutineIndexByComponent = new();

        #region Implementation of ICoroutineSystem

        public Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep)
            => TrackCoroutineOwnership(new Coroutine(this, coroutine, updateMode));

        public Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Entity owner,
            CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep)
            => TrackCoroutineOwnership(new Coroutine(this, coroutine, owner, updateMode));

        public Coroutine CreateCoroutine(IEnumerator<CoroutineInstruction> coroutine, Component owner,
            CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep)
            => TrackCoroutineOwnership(new Coroutine(this, coroutine, owner, updateMode));

        public Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep)
            => StartCoroutine(CreateCoroutine(coroutine, updateMode));

        public Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, Entity owner,
            CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep)
            => StartCoroutine(CreateCoroutine(coroutine, owner, updateMode));

        public Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine, Component owner,
            CoroutineUpdateMode updateMode = CoroutineUpdateMode.VariableTimeStep)
            => StartCoroutine(CreateCoroutine(coroutine, owner, updateMode));

        #endregion

        #region Implementation of ICoroutineGameLoopStep

        public void ProcessCoroutines()
        {
            ProcessCoroutines(new GameTime(GameTime.FixedDeltaTime), _fixedTimeStepContext);
        }

        public void ProcessCoroutines(GameTime gameTime)
        {
            ProcessCoroutines(gameTime, _variableTimeStepContext);
        }

        #endregion

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
            var context = GetContext(coroutine.UpdateMode);
            context.CoroutinesToRemove.Add(coroutine);
        }

        /// <summary>
        ///     Internal API for <see cref="Coroutine" /> class.
        /// </summary>
        internal void OnCoroutineCompleted(Coroutine coroutine)
        {
            var context = GetContext(coroutine.UpdateMode);
            context.CoroutinesToRemove.Add(coroutine);
        }

        internal void OnSwitchToCoroutine(Coroutine from, Coroutine to)
        {
            switch (to.State)
            {
                case CoroutineState.Aborted:
                    throw new InvalidOperationException("Cannot switch to aborted coroutine.");
                case CoroutineState.Completed:
                    throw new InvalidOperationException("Cannot switch to completed coroutine.");
            }

            if (from.UpdateMode != to.UpdateMode)
            {
                throw new InvalidOperationException("Cannot switch to coroutine with different update mode.");
            }

            var context = GetContext(from.UpdateMode);

            if (context.Coroutines.Contains(to))
            {
                throw new InvalidOperationException("Cannot switch to coroutine that is already active.");
            }

            context.SwitchToCoroutines.Add((from, to));
        }

        #endregion

        internal int ActiveCoroutinesCount => _fixedTimeStepContext.Coroutines.Count + _variableTimeStepContext.Coroutines.Count;

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
            var context = GetContext(coroutine.UpdateMode);
            context.JustStartedCoroutines.Add(coroutine);
            coroutine.OnStart();
            return coroutine;
        }

        private void ProcessCoroutines(GameTime gameTime, in Context context)
        {
            context.Coroutines.AddRange(context.JustStartedCoroutines);
            context.JustStartedCoroutines.Clear();

            foreach (var coroutine in context.CoroutinesToRemove)
            {
                RemoveCoroutine(coroutine);
            }

            context.CoroutinesToRemove.Clear();

            foreach (var coroutine in context.Coroutines)
            {
                coroutine.Execute(gameTime);
            }

            foreach (var (from, to) in context.SwitchToCoroutines)
            {
                context.Coroutines.Remove(from);
                context.Coroutines.Add(to);

                if (to.State is CoroutineState.Pending)
                {
                    to.OnStart();
                }
            }

            context.SwitchToCoroutines.Clear();
        }

        private void RemoveCoroutine(Coroutine coroutine)
        {
            var context = GetContext(coroutine.UpdateMode);
            context.Coroutines.Remove(coroutine);

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

        private Context GetContext(CoroutineUpdateMode updateMode) => updateMode switch
        {
            CoroutineUpdateMode.FixedTimeStep => _fixedTimeStepContext,
            CoroutineUpdateMode.VariableTimeStep => _variableTimeStepContext,
            _ => throw new ArgumentOutOfRangeException()
        };

        private readonly struct Context
        {
            public readonly List<Coroutine> Coroutines;
            public readonly List<Coroutine> JustStartedCoroutines;
            public readonly HashSet<Coroutine> CoroutinesToRemove;
            public readonly List<(Coroutine from, Coroutine to)> SwitchToCoroutines;

            public static Context Create() => new(
                new List<Coroutine>(),
                new List<Coroutine>(),
                new HashSet<Coroutine>(),
                new List<(Coroutine from, Coroutine to)>()
            );

            private Context(
                List<Coroutine> coroutines,
                List<Coroutine> justStartedCoroutines,
                HashSet<Coroutine> coroutinesToRemove,
                List<(Coroutine from, Coroutine to)> switchToCoroutines)
            {
                Coroutines = coroutines;
                JustStartedCoroutines = justStartedCoroutines;
                CoroutinesToRemove = coroutinesToRemove;
                SwitchToCoroutines = switchToCoroutines;
            }
        }
    }
}