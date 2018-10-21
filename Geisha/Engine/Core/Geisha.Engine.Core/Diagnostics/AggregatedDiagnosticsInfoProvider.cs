using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.Diagnostics
{
    /// <summary>
    ///     Aggregates all <see cref="IDiagnosticsInfoProvider" /> implementations and provides single point of querying for
    ///     all available <see cref="DiagnosticsInfo" />.
    /// </summary>
    public interface IAggregatedDiagnosticsInfoProvider
    {
        IEnumerable<DiagnosticsInfo> GetDiagnosticsInfo();
    }

    public class AggregatedDiagnosticsInfoProvider : IAggregatedDiagnosticsInfoProvider
    {
        private readonly IEnumerable<IDiagnosticsInfoProvider> _diagnosticsInfoProviders;

        public AggregatedDiagnosticsInfoProvider(IEnumerable<IDiagnosticsInfoProvider> diagnosticsInfoProviders)
        {
            _diagnosticsInfoProviders = diagnosticsInfoProviders;
        }

        public IEnumerable<DiagnosticsInfo> GetDiagnosticsInfo()
        {
            return _diagnosticsInfoProviders.SelectMany(p => p.GetDiagnosticsInfo());
        }
    }
}