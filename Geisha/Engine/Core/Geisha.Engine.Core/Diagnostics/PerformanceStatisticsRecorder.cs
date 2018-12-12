using System;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface IPerformanceStatisticsRecorder
    {
        void RecordFrame();
        void RecordSystemExecution(IFixedTimeStepSystem system, Action action);
        void RecordSystemExecution(IVariableTimeStepSystem system, Action action);
    }

    internal sealed class PerformanceStatisticsRecorder : IPerformanceStatisticsRecorder
    {
        public void RecordFrame()
        {
            throw new NotImplementedException();
        }

        public void RecordSystemExecution(IFixedTimeStepSystem system, Action action)
        {
            action();
        }

        public void RecordSystemExecution(IVariableTimeStepSystem system, Action action)
        {
            action();
        }
    }
}