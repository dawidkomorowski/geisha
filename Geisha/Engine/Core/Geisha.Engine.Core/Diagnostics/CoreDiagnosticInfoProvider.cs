using System.Collections.Generic;
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
            // TODO Add diagnostics about systems share in frame time.

            return diagnosticInfo;
        }

        private DiagnosticInfo GetFpsDiagnosticInfo()
        {
            return new DiagnosticInfo {Name = "FPS", Value = _performanceStatisticsProvider.AvgFps};
        }

        private DiagnosticInfo GetFrameTimeDiagnosticInfo()
        {
            return new DiagnosticInfo {Name = "FrameTime", Value = _performanceStatisticsProvider.FrameTime};
        }

        private DiagnosticInfo GetTotalFramesDiagnosticInfo()
        {
            return new DiagnosticInfo {Name = "TotalFrames", Value = _performanceStatisticsProvider.TotalFrames};
        }

        private DiagnosticInfo GetTotalTimeDiagnosticInfo()
        {
            return new DiagnosticInfo {Name = "TotalTime", Value = _performanceStatisticsProvider.TotalTime};
        }

        private DiagnosticInfo GetRootEntitiesCountDiagnosticInfo()
        {
            return new DiagnosticInfo {Name = "RootEntitiesCount", Value = _rootEntitiesCount};
        }

        private DiagnosticInfo GetAllEntitiesCountDiagnosticInfo()
        {
            return new DiagnosticInfo {Name = "AllEntitiesCount", Value = _allEntitiesCount};
        }
    }
}