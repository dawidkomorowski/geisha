using System.Collections.Generic;
using Geisha.Engine.Core.Diagnostics;

namespace Geisha.Engine.Core
{
    public interface IEngine
    {
        bool IsScheduledForShutdown { get; }
        void Update();
    }

    internal sealed class Engine : IEngine
    {
        private readonly IEngineManager _engineManager;
        private readonly IGameLoop _gameLoop;

        public Engine(IGameLoop gameLoop, IEngineManager engineManager, IAggregatedDiagnosticInfoRegistry aggregatedDiagnosticInfoRegistry,
            IEnumerable<IDiagnosticInfoProvider> diagnosticInfoProviders)
        {
            _gameLoop = gameLoop;
            _engineManager = engineManager;

            foreach (var diagnosticInfoProvider in diagnosticInfoProviders)
            {
                aggregatedDiagnosticInfoRegistry.Register(diagnosticInfoProvider);
            }
        }

        public bool IsScheduledForShutdown => _engineManager.IsEngineScheduledForShutdown;

        public void Update()
        {
            _gameLoop.Update();
        }
    }
}