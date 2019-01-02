using System.Collections.Generic;

namespace Geisha.Engine.Core.Diagnostics
{
    /// <summary>
    ///     Defines provider of <see cref="DiagnosticInfo" /> for single area. Any subsystem that would like to supply custom
    ///     <see cref="DiagnosticInfo" /> should provide and export implementation of this interface.
    /// </summary>
    /// <remarks>
    ///     Concrete implementation of <see cref="IDiagnosticInfoProvider" /> is responsible for filtering
    ///     <see cref="DiagnosticInfo" /> that it would like to supply.
    ///     Therefore, if supplying of particular <see cref="DiagnosticInfo" /> depends on some configuration, then result of
    ///     <see cref="GetDiagnosticInfo" /> should return only <see cref="DiagnosticInfo" /> relevant at that time.
    /// </remarks>
    public interface IDiagnosticInfoProvider
    {
        // TODO Add documentation.
        IEnumerable<DiagnosticInfo> GetDiagnosticInfo();
    }
}