using System.Collections.Generic;

namespace Geisha.Engine.Core.Diagnostics
{
    /// <summary>
    /// Aggregates all <see cref="IDiagnosticsInfoProvider"/> implementations and provides single point of querying for all available <see cref="DiagnosticsInfo"/>.
    /// </summary>
    public interface IAggregatedDiagnosticsInfoProvider
    {
        IEnumerable<DiagnosticsInfo> GetDiagnosticsInfo();
    }
}