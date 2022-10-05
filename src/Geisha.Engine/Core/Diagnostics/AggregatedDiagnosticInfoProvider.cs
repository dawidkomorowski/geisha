using System;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.Diagnostics
{
    /// <summary>
    ///     Aggregates all <see cref="IDiagnosticInfoProvider" /> implementations and provides single point for querying all
    ///     available <see cref="DiagnosticInfo" />.
    /// </summary>
    public interface IAggregatedDiagnosticInfoProvider
    {
        /// <summary>
        ///     Returns all diagnostic info from all providers.
        /// </summary>
        /// <returns>Collection of <see cref="DiagnosticInfo" />.</returns>
        IEnumerable<DiagnosticInfo> GetAllDiagnosticInfo();
    }

    internal sealed class AggregatedDiagnosticInfoProvider : IAggregatedDiagnosticInfoProvider
    {
        private readonly List<IDiagnosticInfoProvider> _diagnosticInfoProviders = new();
        private bool _isInitialized;

        public void Initialize(IEnumerable<IDiagnosticInfoProvider> diagnosticInfoProviders)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(AggregatedDiagnosticInfoProvider)} is already initialized.");
            }

            _diagnosticInfoProviders.AddRange(diagnosticInfoProviders);

            _isInitialized = true;
        }

        public IEnumerable<DiagnosticInfo> GetAllDiagnosticInfo()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(AggregatedDiagnosticInfoProvider)} is not initialized.");
            }

            return _diagnosticInfoProviders.SelectMany(p => p.GetDiagnosticInfo());
        }
    }
}