using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.Coroutines
{
    public interface ICoroutineSystem
    {
        void StartCoroutine(IEnumerator<Coroutine> coroutine);
    }

    internal sealed class CoroutineSystem : ICoroutineSystem
    {
        private readonly List<IEnumerator<Coroutine>> _coroutines = new();
        public void StartCoroutine(IEnumerator<Coroutine> coroutine)
        {
            _coroutines.Add(coroutine);
        }

        public void ProcessCoroutines()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.MoveNext();
            }
        }
    }
}