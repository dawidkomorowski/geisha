using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Core
{
    // TODO then used GameTime object to provide time information (time from begining, delta time, count of frames?)
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

    internal interface IGameTimeProvider
    {
        GameTime GetGameTime();
    }

    [Export(typeof(IGameTimeProvider))]
    internal class GameTimeProvider : IGameTimeProvider
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        [ImportingConstructor]
        public GameTimeProvider(IConfigurationManager configurationManager)
        {
            GameTime.StartUpTime = DateTime.Now;
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(1.0d / configurationManager.GetConfiguration<CoreConfiguration>().FixedUpdatesPerSecond);
        }

        public GameTime GetGameTime()
        {
            var deltaTime = _stopwatch.Elapsed;
            _stopwatch.Restart();
            return new GameTime(deltaTime);
        }
    }
}