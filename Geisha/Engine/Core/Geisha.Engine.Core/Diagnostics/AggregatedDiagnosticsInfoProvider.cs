using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Geisha.Engine.Core.Diagnostics
{
    [Export(typeof(IAggregatedDiagnosticsInfoProvider))]
    public class AggregatedDiagnosticsInfoProvider : IAggregatedDiagnosticsInfoProvider
    {
        private readonly IEnumerable<IDiagnosticsInfoProvider> _diagnosticsInfoProviders;

        [ImportingConstructor]
        public AggregatedDiagnosticsInfoProvider([ImportMany] IEnumerable<IDiagnosticsInfoProvider> diagnosticsInfoProviders)
        {
            _diagnosticsInfoProviders = diagnosticsInfoProviders;
        }

        public IEnumerable<DiagnosticsInfo> GetDiagnosticsInfo()
        {
            return _diagnosticsInfoProviders.SelectMany(p => p.GetDiagnosticsInfo());
        }
    }
}