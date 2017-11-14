using System.ComponentModel.Composition;

namespace Geisha.Engine.Core
{
    public interface IEngineManager
    {
        bool IsEngineScheduledForShutdown { get; }

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