using System.Collections.Generic;

namespace Geisha.Engine.Core.Diagnostics
{
    /// <summary>
    ///     Represents single <see cref="DiagnosticsInfo" /> provider. Any subsystem that would like to supply custom
    ///     <see cref="DiagnosticsInfo" /> should provide and export implementation of this interface.
    /// </summary>
    /// <remarks>
    ///     Concrete implementation of <see cref="IDiagnosticsInfoProvider" /> is responsible for filtering
    ///     <see cref="DiagnosticsInfo" /> that it would like to supply.
    ///     Therefore, if supplying of particular <see cref="DiagnosticsInfo" /> depends on some configuration, then result of
    ///     <see cref="GetDiagnosticsInfo" /> should return only <see cref="DiagnosticsInfo" /> relevant at that time.
    /// </remarks>
    public interface IDiagnosticsInfoProvider
    {
        IEnumerable<DiagnosticsInfo> GetDiagnosticsInfo();
    }
}