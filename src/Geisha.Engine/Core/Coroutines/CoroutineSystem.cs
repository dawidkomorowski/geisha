using System.Collections.Generic;

namespace Geisha.Engine.Core.Coroutines
{
    public interface ICoroutineSystem
    {
        Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine);
    }

    internal sealed class CoroutineSystem : ICoroutineSystem
    {
        private readonly List<Coroutine> _coroutines = new();

        public Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine)
        {
            var newCoroutine = Coroutine.Create(coroutine);
            _coroutines.Add(newCoroutine);
            newCoroutine.OnStart();

            return newCoroutine;
        }

        public void ProcessCoroutines(GameTime gameTime)
        {
            _coroutines.RemoveAll(c => c.State is CoroutineState.Completed or CoroutineState.Aborted);

            foreach (var coroutine in _coroutines)
            {
                coroutine.Execute(gameTime);
            }
        }
    }
}