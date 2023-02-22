using System.Collections.Generic;

namespace Geisha.Engine.Core.Coroutines
{
    // TODO Better handling of SwitchTo?
    // TODO OnStart() when switching to coroutine?
    // TODO FixedTimeStep coroutine execution
    // TODO Entity and Component ownership
    // TODO Refactor?
    // TODO How should exceptions work in coroutines?
    public interface ICoroutineSystem
    {
        Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine);
    }

    internal sealed class CoroutineSystem : ICoroutineSystem
    {
        private readonly List<Coroutine> _coroutines = new();
        private readonly List<Coroutine> _pendingCoroutines = new();

        public Coroutine StartCoroutine(IEnumerator<CoroutineInstruction> coroutine)
        {
            var newCoroutine = Coroutine.Create(coroutine);
            _pendingCoroutines.Add(newCoroutine);
            newCoroutine.OnStart();

            return newCoroutine;
        }

        public void ProcessCoroutines(GameTime gameTime)
        {
            _coroutines.AddRange(_pendingCoroutines);
            _pendingCoroutines.Clear();
            _coroutines.RemoveAll(c => c.State is CoroutineState.Completed or CoroutineState.Aborted);

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
    }
}