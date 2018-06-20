using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Geisha.Common;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Core
{
    internal interface IGameTimeProvider
    {
        GameTime GetGameTime();
    }

    [Export(typeof(IGameTimeProvider))]
    internal class GameTimeProvider : IGameTimeProvider
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        [ImportingConstructor]
        public GameTimeProvider(IConfigurationManager configurationManager, IDateTimeProvider dateTimeProvider)
        {
            GameTime.StartUpTime = dateTimeProvider.Now();
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(1.0d / configurationManager.GetConfiguration<CoreConfiguration>().FixedUpdatesPerSecond);
        }

        public GameTime GetGameTime()
        {
            GameTime.FramesSinceStartUp++;

            var deltaTime = _stopwatch.Elapsed;
            _stopwatch.Restart();
            return new GameTime(deltaTime);
        }
    }
}