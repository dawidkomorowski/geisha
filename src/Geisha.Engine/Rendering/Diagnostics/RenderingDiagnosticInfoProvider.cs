using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering.Diagnostics;

internal interface IRenderingDiagnosticInfoProvider
{
    void UpdateDiagnostics(RenderingStatistics statistics);
}

internal sealed class RenderingDiagnosticInfoProvider : IRenderingDiagnosticInfoProvider, IDiagnosticInfoProvider
{
    private readonly RenderingConfiguration _configuration;
    private RenderingStatistics _statistics;

    public RenderingDiagnosticInfoProvider(RenderingConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void UpdateDiagnostics(RenderingStatistics statistics)
    {
        _statistics = statistics;
    }

    public IEnumerable<DiagnosticInfo> GetDiagnosticInfo()
    {
        return _configuration.ShowRenderingStatistics
            ? new[] { new DiagnosticInfo("DrawCalls", _statistics.DrawCalls.ToString()) }
            : Enumerable.Empty<DiagnosticInfo>();
    }
}