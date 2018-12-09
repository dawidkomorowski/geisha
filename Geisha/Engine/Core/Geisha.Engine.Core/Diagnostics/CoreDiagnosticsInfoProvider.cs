using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface ICoreDiagnosticsInfoProvider
    {
        void UpdateDiagnostics(Scene scene);
    }

    internal class CoreDiagnosticsInfoProvider : ICoreDiagnosticsInfoProvider, IDiagnosticsInfoProvider
    {
        private readonly IConfigurationManager _configurationManager;
        private readonly IPerformanceMonitor _performanceMonitor;
        private int _allEntitiesCount;
        private int _rootEntitiesCount;

        public CoreDiagnosticsInfoProvider(IConfigurationManager configurationManager, IPerformanceMonitor performanceMonitor)
        {
            _configurationManager = configurationManager;
            _performanceMonitor = performanceMonitor;
        }

        public void UpdateDiagnostics(Scene scene)
        {
            _rootEntitiesCount = scene.RootEntities.Count;
            _allEntitiesCount = scene.AllEntities.Count();
        }

        public IEnumerable<DiagnosticsInfo> GetDiagnosticsInfo()
        {
            var coreConfiguration = _configurationManager.GetConfiguration<CoreConfiguration>();
            var diagnosticsInfo = new List<DiagnosticsInfo>();

            if (coreConfiguration.ShowFps) diagnosticsInfo.Add(GetFpsDiagnosticsInfo());
            if (coreConfiguration.ShowFrameTime) diagnosticsInfo.Add(GetFrameTimeDiagnosticsInfo());
            if (coreConfiguration.ShowTotalFrames) diagnosticsInfo.Add(GetTotalFramesDiagnosticsInfo());
            if (coreConfiguration.ShowTotalTime) diagnosticsInfo.Add(GetTotalTimeDiagnosticsInfo());
            if (coreConfiguration.ShowRootEntitiesCount) diagnosticsInfo.Add(GetRootEntitiesCountDiagnosticsInfo());
            if (coreConfiguration.ShowAllEntitiesCount) diagnosticsInfo.Add(GetAllEntitiesCountDiagnosticsInfo());

            return diagnosticsInfo;
        }

        private static DiagnosticsInfo GetFpsDiagnosticsInfo()
        {
            return new DiagnosticsInfo {Name = "FPS", Value = PerformanceMonitor.RealFps};
        }

        private static DiagnosticsInfo GetFrameTimeDiagnosticsInfo()
        {
            return new DiagnosticsInfo {Name = "FrameTime", Value = PerformanceMonitor.FrameTime};
        }

        private DiagnosticsInfo GetTotalFramesDiagnosticsInfo()
        {
            return new DiagnosticsInfo {Name = "TotalFrames", Value = _performanceMonitor.TotalFrames};
        }

        private static DiagnosticsInfo GetTotalTimeDiagnosticsInfo()
        {
            return new DiagnosticsInfo {Name = "TotalTime", Value = PerformanceMonitor.TotalTime};
        }

        private DiagnosticsInfo GetRootEntitiesCountDiagnosticsInfo()
        {
            return new DiagnosticsInfo {Name = "RootEntitiesCount", Value = _rootEntitiesCount};
        }

        private DiagnosticsInfo GetAllEntitiesCountDiagnosticsInfo()
        {
            return new DiagnosticsInfo {Name = "AllEntitiesCount", Value = _allEntitiesCount};
        }
    }
}