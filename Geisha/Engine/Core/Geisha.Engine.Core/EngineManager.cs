using System.ComponentModel.Composition;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Provides client API to control the engine.
    /// </summary>
    public interface IEngineManager
    {
        /// <summary>
        ///     Indicates whether engine is scheduled for shutdown.
        /// </summary>
        bool IsEngineScheduledForShutdown { get; }

        /// <summary>
        ///     Schedules engine shutdown in the end of current frame.
        /// </summary>
        void ScheduleEngineShutdown();
    }


    [Export(typeof(IEngineManager))]
    internal class EngineManager : IEngineManager
    {
        public bool IsEngineScheduledForShutdown { get; private set; }

        public void ScheduleEngineShutdown()
        {
            IsEngineScheduledForShutdown = true;
        }
    }
}