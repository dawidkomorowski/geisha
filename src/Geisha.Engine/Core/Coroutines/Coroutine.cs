using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Coroutines
{
    /// <summary>
    ///     Represents coroutine managed by coroutine system. <see cref="Coroutine" /> class provides APIs to control execution
    ///     of coroutine. To create or start new <see cref="Coroutine" /> use APIs provided by <see cref="ICoroutineSystem" />.
    /// </summary>
    public sealed class Coroutine
    {
        private readonly CoroutineSystem _coroutineSystem;
        private readonly Stack<IEnumerator<CoroutineInstruction>> _callStack = new();
        private CoroutineInstruction _instruction = new WaitForNextFrameCoroutineInstruction();

        internal Coroutine(CoroutineSystem coroutineSystem, IEnumerator<CoroutineInstruction> coroutine, CoroutineUpdateMode updateMode)
        {
            _coroutineSystem = coroutineSystem;
            _callStack.Push(coroutine);
            UpdateMode = updateMode;
        }

        internal Coroutine(CoroutineSystem coroutineSystem, IEnumerator<CoroutineInstruction> coroutine, Entity owner, CoroutineUpdateMode updateMode)
            : this(coroutineSystem, coroutine, updateMode)
        {
            OwnerEntity = owner;
        }

        internal Coroutine(CoroutineSystem coroutineSystem, IEnumerator<CoroutineInstruction> coroutine, Component owner, CoroutineUpdateMode updateMode)
            : this(coroutineSystem, coroutine, updateMode)
        {
            OwnerEntity = owner.Entity;
            OwnerComponent = owner;
        }

        /// <summary>
        ///     Creates Call instruction that allows to call from current coroutine into another coroutine.
        /// </summary>
        /// <param name="coroutine">Enumerator providing the coroutine implementation that is to be called.</param>
        /// <returns><see cref="CoroutineInstruction" /> representing Call instruction.</returns>
        /// <remarks>
        ///     Call instruction is similar to regular function call. It suspends execution of the calling coroutine and executes
        ///     the callee. Once the callee coroutine completes, execution of the caller is resumed.
        /// </remarks>
        public static CoroutineInstruction Call(IEnumerator<CoroutineInstruction> coroutine)
        {
            return new CallCoroutineInstruction(coroutine);
        }

        /// <summary>
        ///     Creates SwitchTo instruction that allows to switch execution from current coroutine to another coroutine.
        /// </summary>
        /// <param name="coroutine"><see cref="Coroutine" /> instance to switch execution to.</param>
        /// <returns><see cref="CoroutineInstruction" /> representing SwitchTo instruction.</returns>
        /// <remarks>
        ///     <para>
        ///         SwitchTo instruction suspends execution of the current coroutine and switches execution to target coroutine.
        ///         In contrast to Call instruction there is no caller-callee relation therefore once the target coroutine
        ///         completes
        ///         the originating coroutine is not resumed unless explicitly there is a switch back.
        ///     </para>
        ///     <para>
        ///         SwitchTo instruction allows to freely switch between coroutines which allows to easily implement
        ///         consumer-producer pattern or a state machine.
        ///     </para>
        /// </remarks>
        public static CoroutineInstruction SwitchTo(Coroutine coroutine)
        {
            return new SwitchToCoroutineInstruction(coroutine);
        }

        /// <summary>
        ///     Creates WaitForNextFrame instruction that allows to suspend execution of current coroutine until next frame.
        /// </summary>
        /// <returns><see cref="CoroutineInstruction" /> representing WaitForNextFrame instruction.</returns>
        /// <remarks>
        ///     <para>
        ///         WaitForNextFrame instruction is primary way of spreading the execution of coroutine code over multiple
        ///         frames. Use it in loops or between chunks of code in order to make progress of the coroutine tasks and at the
        ///         same time let the engine process other systems.
        ///     </para>
        ///     <para>
        ///         Concept of next frame depends on selected <see cref="CoroutineUpdateMode" />. If the coroutine
        ///         <see cref="UpdateMode" /> is <see cref="CoroutineUpdateMode.VariableTimeStep" /> the next frame is next
        ///         variable time step update. If the coroutine <see cref="UpdateMode" /> is
        ///         <see cref="CoroutineUpdateMode.FixedTimeStep" /> the next frame is next fixed time step update.
        ///     </para>
        /// </remarks>
        public static CoroutineInstruction WaitForNextFrame()
        {
            return new WaitForNextFrameCoroutineInstruction();
        }

        /// <summary>
        ///     Creates Wait instruction that allows to suspend execution of current coroutine for specified timespan.
        /// </summary>
        /// <param name="waitTime"><see cref="TimeSpan" /> of the wait.</param>
        /// <returns><see cref="CoroutineInstruction" /> representing Wait instruction.</returns>
        /// <remarks>
        ///     Actual time of wait may vary from the specified <paramref name="waitTime" />. It is due to discrete nature of
        ///     finite number of updates per second. Each update moves the logical clock by elapsed delta time or fixed delta time,
        ///     depending on selected <see cref="CoroutineUpdateMode" />. Wait instruction guarantees that at least specified
        ///     <paramref name="waitTime" /> elapsed before resuming the coroutine. However the actual elapsed time may be longer.
        /// </remarks>
        public static CoroutineInstruction Wait(TimeSpan waitTime)
        {
            return new WaitCoroutineInstruction(waitTime);
        }

        /// <summary>
        ///     Creates WaitUntil instruction that allows to suspend execution of current coroutine until specified condition is
        ///     met.
        /// </summary>
        /// <param name="condition">Predicate defining condition to meet.</param>
        /// <returns><see cref="CoroutineInstruction" /> representing WaitUntil instruction.</returns>
        public static CoroutineInstruction WaitUntil(Func<bool> condition)
        {
            return new WaitUntilCoroutineInstruction(condition);
        }

        /// <summary>
        ///     Gets <see cref="Entity" /> that is owner of this <see cref="Coroutine" /> or <c>null</c> if
        ///     no <see cref="Entity" /> is owner of this <see cref="Coroutine" />.
        /// </summary>
        public Entity? OwnerEntity { get; }

        /// <summary>
        ///     Gets <see cref="Component" /> that is owner of this <see cref="Coroutine" /> or <c>null</c> if
        ///     no <see cref="Component" /> is owner of this <see cref="Coroutine" />.
        /// </summary>
        public Component? OwnerComponent { get; }

        /// <summary>
        ///     Gets state of this <see cref="Coroutine" />.
        /// </summary>
        public CoroutineState State { get; private set; } = CoroutineState.Pending;

        /// <summary>
        ///     Gets update mode of this <see cref="Coroutine" />.
        /// </summary>
        public CoroutineUpdateMode UpdateMode { get; }

        /// <summary>
        ///     Pauses execution of the <see cref="Coroutine" />. Paused coroutine is not being executed until it is resumed.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The caller attempted to pause <see cref="Coroutine" /> that is in <see cref="CoroutineState.Pending" />,
        ///     <see cref="CoroutineState.Completed" /> or <see cref="CoroutineState.Aborted" /> state.
        /// </exception>
        /// <seealso cref="Resume" />
        /// <seealso cref="Abort"/>
        public void Pause()
        {
            if (State is CoroutineState.Pending or CoroutineState.Completed or CoroutineState.Aborted)
            {
                throw new InvalidOperationException($"Coroutine in state '{State}' cannot be paused.");
            }

            State = CoroutineState.Paused;
        }

        /// <summary>
        ///     Resumes execution of paused <see cref="Coroutine" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The caller attempted to resume <see cref="Coroutine" /> that is in
        ///     <see cref="CoroutineState.Pending" />, <see cref="CoroutineState.Completed" /> or
        ///     <see cref="CoroutineState.Aborted" /> state.
        /// </exception>
        /// <seealso cref="Pause" />
        /// <seealso cref="Abort"/>
        public void Resume()
        {
            if (State is CoroutineState.Pending or CoroutineState.Completed or CoroutineState.Aborted)
            {
                throw new InvalidOperationException($"Coroutine in state '{State}' cannot be resumed.");
            }

            State = CoroutineState.Running;
        }

        /// <summary>
        ///     Aborts the <see cref="Coroutine" />. Aborted coroutine is permanently terminated.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The caller attempted to abort <see cref="Coroutine" /> that is in
        ///     <see cref="CoroutineState.Completed" /> state.
        /// </exception>
        /// <seealso cref="Pause" />
        /// <seealso cref="Resume" />
        public void Abort()
        {
            switch (State)
            {
                case CoroutineState.Aborted:
                    return;
                case CoroutineState.Completed:
                    throw new InvalidOperationException($"Coroutine in state '{State}' cannot be aborted.");
                default:
                    State = CoroutineState.Aborted;
                    _coroutineSystem.OnCoroutineAborted(this);
                    break;
            }
        }

        internal void OnStart()
        {
            if (State is not CoroutineState.Pending)
            {
                throw new InvalidOperationException($"Coroutine in state '{State}' cannot be started.");
            }

            State = CoroutineState.Running;
        }

        internal void Execute(GameTime gameTime)
        {
            if (State != CoroutineState.Running) return;
            if (!_instruction.IsCompleted(gameTime)) return;

            var coroutine = _callStack.Peek();

            while (!coroutine.MoveNext())
            {
                _callStack.Pop();

                if (_callStack.Count == 0)
                {
                    State = CoroutineState.Completed;
                    _coroutineSystem.OnCoroutineCompleted(this);
                    return;
                }

                coroutine = _callStack.Peek();
            }

            _instruction = coroutine.Current;
            _instruction.Execute(this);
        }

        internal void HandleCallInstruction(CallCoroutineInstruction instruction)
        {
            _callStack.Push(instruction.Coroutine);
        }

        internal void HandleSwitchToInstruction(SwitchToCoroutineInstruction instruction)
        {
            _coroutineSystem.OnSwitchToCoroutine(this, instruction.Coroutine);
        }
    }
}