﻿using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Coroutines
{
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

        public static CoroutineInstruction Call(IEnumerator<CoroutineInstruction> coroutine)
        {
            return new CallCoroutineInstruction(coroutine);
        }

        public static CoroutineInstruction SwitchTo(Coroutine coroutine)
        {
            return new SwitchToCoroutineInstruction(coroutine);
        }

        public static CoroutineInstruction WaitForNextFrame()
        {
            return new WaitForNextFrameCoroutineInstruction();
        }

        public static CoroutineInstruction Wait(TimeSpan waitTime)
        {
            return new WaitCoroutineInstruction(waitTime);
        }

        public static CoroutineInstruction WaitUntil(Func<bool> condition)
        {
            return new WaitUntilCoroutineInstruction(condition);
        }

        public Entity? OwnerEntity { get; }
        public Component? OwnerComponent { get; }
        public CoroutineState State { get; private set; } = CoroutineState.Pending;
        public CoroutineUpdateMode UpdateMode { get; }

        public void Pause()
        {
            if (State is CoroutineState.Pending or CoroutineState.Completed or CoroutineState.Aborted)
            {
                throw new InvalidOperationException($"Coroutine in state '{State}' cannot be paused.");
            }

            State = CoroutineState.Paused;
        }

        public void Resume()
        {
            if (State is CoroutineState.Pending or CoroutineState.Completed or CoroutineState.Aborted)
            {
                throw new InvalidOperationException($"Coroutine in state '{State}' cannot be resumed.");
            }

            State = CoroutineState.Running;
        }

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