using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Geisha.Engine.Core
{
    // TODO Replace IDeltaTimeProvider and IFixedDeltaTimeProvider with single IGameTimeProvider (or something similar)?
    public interface IDeltaTimeProvider
    {
        double GetDeltaTime();
    }

    [Export(typeof(IDeltaTimeProvider))]
    public class DeltaTimeProvider : IDeltaTimeProvider
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public double GetDeltaTime()
        {
            var elapsed = _stopwatch.Elapsed;
            _stopwatch.Restart();
            return elapsed.TotalSeconds;
        }
    }
}