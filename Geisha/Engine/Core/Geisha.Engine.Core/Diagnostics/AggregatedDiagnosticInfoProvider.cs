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

    internal interface IAggregatedDiagnosticInfoRegistry
    {
        void Register(IDiagnosticInfoProvider diagnosticInfoProvider);
    }

    internal class AggregatedDiagnosticInfoProvider : IAggregatedDiagnosticInfoProvider, IAggregatedDiagnosticInfoRegistry
    {
        private readonly List<IDiagnosticInfoProvider> _diagnosticInfoProviders = new List<IDiagnosticInfoProvider>();

        public IEnumerable<DiagnosticInfo> GetAllDiagnosticInfo()
        {
            return _diagnosticInfoProviders.SelectMany(p => p.GetDiagnosticInfo());
        }

        public void Register(IDiagnosticInfoProvider diagnosticInfoProvider)
        {
            _diagnosticInfoProviders.Add(diagnosticInfoProvider);
        }
    }
}