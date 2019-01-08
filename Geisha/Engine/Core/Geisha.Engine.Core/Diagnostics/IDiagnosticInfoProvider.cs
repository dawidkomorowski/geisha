using System.Collections.Generic;

namespace Geisha.Engine.Core.Diagnostics
{
    /// <summary>
    ///     Defines provider of <see cref="DiagnosticInfo" /> for certain area. Any subsystem that would like to supply custom
    ///     <see cref="DiagnosticInfo" /> should provide and register implementation of this interface.
    /// </summary>
    /// <remarks>
    ///     Concrete implementation of <see cref="IDiagnosticInfoProvider" /> is responsible for filtering
    ///     <see cref="DiagnosticInfo" /> that it would like to supply.
    ///     Therefore, if supplying of particular <see cref="DiagnosticInfo" /> depends on some configuration, then result of
    ///     <see cref="GetDiagnosticInfo" /> should return only <see cref="DiagnosticInfo" /> relevant at that time.
    /// </remarks>
    public interface IDiagnosticInfoProvider
    {
        /// <summary>
        ///     Returns all <see cref="DiagnosticInfo" /> supplied by provider.
        /// </summary>
        /// <returns>Collection of <see cref="DiagnosticInfo" />.</returns>
        IEnumerable<DiagnosticInfo> GetDiagnosticInfo();
    }
}