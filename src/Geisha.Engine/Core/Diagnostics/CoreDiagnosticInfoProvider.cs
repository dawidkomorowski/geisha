using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface ICoreDiagnosticInfoProvider
    {
        void UpdateDiagnostics(Scene scene);
    }

    internal sealed class CoreDiagnosticInfoProvider : ICoreDiagnosticInfoProvider, IDiagnosticInfoProvider
    {
        private readonly CoreConfiguration _configuration;
        private readonly IPerformanceStatisticsProvider _performanceStatisticsProvider;
        private int _allEntitiesCount;
        private int _rootEntitiesCount;

        public CoreDiagnosticInfoProvider(CoreConfiguration configuration, IPerformanceStatisticsProvider performanceStatisticsProvider)
        {
            _configuration = configuration;
            _performanceStatisticsProvider = performanceStatisticsProvider;
        }

        public void UpdateDiagnostics(Scene scene)
        {
            _rootEntitiesCount = scene.RootEntities.Count;
            _allEntitiesCount = scene.AllEntities.Count();
        }

        public IEnumerable<DiagnosticInfo> GetDiagnosticInfo()
        {
            var diagnosticInfo = new List<DiagnosticInfo>();

            if (_configuration.ShowFps) diagnosticInfo.Add(GetFpsDiagnosticInfo());
            if (_configuration.ShowFrameTime) diagnosticInfo.Add(GetFrameTimeDiagnosticInfo());
            if (_configuration.ShowTotalFrames) diagnosticInfo.Add(GetTotalFramesDiagnosticInfo());
            if (_configuration.ShowTotalTime) diagnosticInfo.Add(GetTotalTimeDiagnosticInfo());
            if (_configuration.ShowRootEntitiesCount) diagnosticInfo.Add(GetRootEntitiesCountDiagnosticInfo());
            if (_configuration.ShowAllEntitiesCount) diagnosticInfo.Add(GetAllEntitiesCountDiagnosticInfo());
            if (_configuration.ShowSystemsExecutionTimes) diagnosticInfo.AddRange(GetSystemsExecutionTimesDiagnosticInfo());

            return diagnosticInfo;
        }

        private DiagnosticInfo GetFpsDiagnosticInfo()
        {
            return new DiagnosticInfo("FPS", _performanceStatisticsProvider.AvgFps.ToString(CultureInfo.InvariantCulture));
        }

        private DiagnosticInfo GetFrameTimeDiagnosticInfo()
        {
            return new DiagnosticInfo("FrameTime", _performanceStatisticsProvider.FrameTime.ToString());
        }

        private DiagnosticInfo GetTotalFramesDiagnosticInfo()
        {
            return new DiagnosticInfo("TotalFrames", _performanceStatisticsProvider.TotalFrames.ToString());
        }

        private DiagnosticInfo GetTotalTimeDiagnosticInfo()
        {
            return new DiagnosticInfo("TotalTime", _performanceStatisticsProvider.TotalTime.ToString());
        }

        private DiagnosticInfo GetRootEntitiesCountDiagnosticInfo()
        {
            return new DiagnosticInfo("RootEntitiesCount", _rootEntitiesCount.ToString());
        }

        private DiagnosticInfo GetAllEntitiesCountDiagnosticInfo()
        {
            return new DiagnosticInfo("AllEntitiesCount", _allEntitiesCount.ToString());
        }

        private IEnumerable<DiagnosticInfo> GetSystemsExecutionTimesDiagnosticInfo()
        {
            return _performanceStatisticsProvider.GetSystemsExecutionTime()
                .Select(t => new DiagnosticInfo(t.SystemName, $"{t.AvgFrameTime} [{Math.Round(t.AvgFrameTimeShare * 100)}%]"));
        }
    }
}