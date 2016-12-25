using System.Diagnostics;

namespace Geisha.Engine.Core
{
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