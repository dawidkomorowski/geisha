namespace Geisha.Engine.Core.Memory;

/// <summary>
///     Marker interface that enforces unmanaged data at compile time.
/// </summary>
/// <typeparam name="T">
///     Type constrained to <see langword="unmanaged" />. Typically, this is the implementing value type itself.
/// </typeparam>
/// <remarks>
///     <para>
///         Implementing this interface is a lightweight way to guard performance-critical data structures against
///         accidentally introducing managed fields.
///     </para>
///     <para>This interface defines no members and is used only as a compile-time contract.</para>
/// </remarks>
public interface IUnmanaged<T> where T : unmanaged
{
}