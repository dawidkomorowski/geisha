using System;
using System.Diagnostics;
using Geisha.Common;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Interface providing <see cref="GameTime" />.
    /// </summary>
    internal interface IGameTimeProvider
    {
        /// <summary>
        ///     Returns <see cref="GameTime" /> with delta time equal time passed since last call to this method.
        /// </summary>
        /// <returns>
        ///     <see cref="GameTime" /> with delta time equal time passed since last call to this method. First call returns
        ///     <see cref="GameTime" /> with delta time equal zero.
        /// </returns>
        GameTime GetGameTime();
    }

    /// <summary>
    ///     Class providing <see cref="GameTime" />.
    /// </summary>
    internal sealed class GameTimeProvider : IGameTimeProvider
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public GameTimeProvider(IConfigurationManager configurationManager, IDateTimeProvider dateTimeProvider)
        {
            GameTime.DateTimeProvider = dateTimeProvider;
            GameTime.StartUpTime = dateTimeProvider.Now();
            GameTime.FixedDeltaTime = TimeSpan.FromSeconds(1.0d / configurationManager.GetConfiguration<CoreConfiguration>().FixedUpdatesPerSecond);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns <see cref="T:Geisha.Engine.Core.GameTime" /> with delta time equal time passed since last call to this
        ///     method.
        /// </summary>
        public GameTime GetGameTime()
        {
            GameTime.FramesSinceStartUp++;

            var deltaTime = _stopwatch.Elapsed;
            _stopwatch.Restart();
            return new GameTime(deltaTime);
        }
    }
}