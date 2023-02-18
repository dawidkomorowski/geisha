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

            var list = new List<(Coroutine target, Coroutine source)>();
            foreach (var coroutine in _coroutines)
            {
                var targetCoroutine = coroutine.Execute(gameTime);
                if (targetCoroutine is not null)
                {
                    list.Add((targetCoroutine, coroutine));
                }
            }

            foreach (var tuple in list)
            {
                _coroutines.Remove(tuple.source);
                _coroutines.Add(tuple.target);
                tuple.target.OnStart();
            }
        }
    }
}