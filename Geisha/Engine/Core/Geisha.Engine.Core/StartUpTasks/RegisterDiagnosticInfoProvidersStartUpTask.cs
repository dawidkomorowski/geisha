﻿using System.Collections.Generic;
using Geisha.Engine.Core.Diagnostics;

namespace Geisha.Engine.Core.StartUpTasks
{
    internal interface IRegisterDiagnosticInfoProvidersStartUpTask
    {
        void Run();
    }

    internal class RegisterDiagnosticInfoProvidersStartUpTask : IRegisterDiagnosticInfoProvidersStartUpTask
    {
        private readonly IAggregatedDiagnosticInfoRegistry _aggregatedDiagnosticInfoRegistry;
        private readonly IEnumerable<IDiagnosticInfoProvider> _diagnosticInfoProviders;

        public RegisterDiagnosticInfoProvidersStartUpTask(IAggregatedDiagnosticInfoRegistry aggregatedDiagnosticInfoRegistry,
            IEnumerable<IDiagnosticInfoProvider> diagnosticInfoProviders)
        {
            _aggregatedDiagnosticInfoRegistry = aggregatedDiagnosticInfoRegistry;
            _diagnosticInfoProviders = diagnosticInfoProviders;
        }

        public void Run()
        {
            foreach (var diagnosticInfoProvider in _diagnosticInfoProviders)
            {
                _aggregatedDiagnosticInfoRegistry.Register(diagnosticInfoProvider);
            }
        }
    }
}