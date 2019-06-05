using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface ICoreDiagnosticInfoProvider
    {
        void UpdateDiagnostics(Scene scene);
    }

    internal sealed class CoreDiagnosticInfoProvider : ICoreDiagnosticInfoProvider, IDiagnosticInfoProvider
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IPerformanceStatisticsProvider _performanceStatisticsProvider;
        private int _allEntitiesCount;
        private int _rootEntitiesCount;

        public CoreDiagnosticInfoProvider(IConfigurationManager configurationManager, IPerformanceStatisticsProvider performanceStatisticsProvider)
        {
            _configurationManager = configurationManager;
            _performanceStatisticsProvider = performanceStatisticsProvider;
        }

        public void UpdateDiagnostics(Scene scene)
        {
            _rootEntitiesCount = scene.RootEntities.Count;
            _allEntitiesCount = scene.AllEntities.Count();
        }

        public IEnumerable<DiagnosticInfo> GetDiagnosticInfo()
        {
            var coreConfiguration = _configurationManager.GetConfiguration<CoreConfiguration>();
            var diagnosticInfo = new List<DiagnosticInfo>();

            if (coreConfiguration.ShowFps) diagnosticInfo.Add(GetFpsDiagnosticInfo());
            if (coreConfiguration.ShowFrameTime) diagnosticInfo.Add(GetFrameTimeDiagnosticInfo());
            if (coreConfiguration.ShowTotalFrames) diagnosticInfo.Add(GetTotalFramesDiagnosticInfo());
            if (coreConfiguration.ShowTotalTime) diagnosticInfo.Add(GetTotalTimeDiagnosticInfo());
            if (coreConfiguration.ShowRootEntitiesCount) diagnosticInfo.Add(GetRootEntitiesCountDiagnosticInfo());
            if (coreConfiguration.ShowAllEntitiesCount) diagnosticInfo.Add(GetAllEntitiesCountDiagnosticInfo());
            if (coreConfiguration.ShowSystemsExecutionTimes) diagnosticInfo.AddRange(GetSystemsExecutionTimesDiagnosticInfo());

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