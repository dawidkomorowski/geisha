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
        // TODO Add documentation.
        IEnumerable<DiagnosticInfo> GetAllDiagnosticInfo();
    }

    internal class AggregatedDiagnosticInfoProvider : IAggregatedDiagnosticInfoProvider
    {
        private readonly IEnumerable<IDiagnosticInfoProvider> _diagnosticInfoProviders;

        public AggregatedDiagnosticInfoProvider(IEnumerable<IDiagnosticInfoProvider> diagnosticInfoProviders)
        {
            _diagnosticInfoProviders = diagnosticInfoProviders;
        }

        public IEnumerable<DiagnosticInfo> GetAllDiagnosticInfo()
        {
            return _diagnosticInfoProviders.SelectMany(p => p.GetDiagnosticInfo());
        }
    }
}