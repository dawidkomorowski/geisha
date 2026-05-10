using System.Collections.Immutable;

namespace Geisha.Engine.Input.Mapping;

/// <summary>
///     Defines mapping of hardware input sources to logical actions and axes.
/// </summary>
public sealed class InputMapping
{
    /// <summary>
    ///     Creates a new <see cref="InputMappingBuilder" /> used to define mapping of hardware input sources to logical actions
    ///     and axes.
    /// </summary>
    /// <returns>New <see cref="InputMappingBuilder" /> instance.</returns>
    public static InputMappingBuilder CreateBuilder() => new();

    /// <summary>
    ///     List of action mappings.
    /// </summary>
    public ImmutableArray<ActionMapping> ActionMappings { get; init; } = ImmutableArray<ActionMapping>.Empty;

    /// <summary>
    ///     List of axis mappings.
    /// </summary>
    public ImmutableArray<AxisMapping> AxisMappings { get; init; } = ImmutableArray<AxisMapping>.Empty;
}

/// <summary>
///     Defines mapping of list of hardware actions to a single logical action.
/// </summary>
public readonly record struct ActionMapping()
{
    /// <summary>
    ///     Name of action.
    /// </summary>
    public string ActionName { get; init; } = string.Empty;

    /// <summary>
    ///     List of hardware actions this logical action is based on.
    /// </summary>
    public ImmutableArray<HardwareAction> HardwareActions { get; init; } = ImmutableArray<HardwareAction>.Empty;
}

/// <summary>
///     Defines mapping of list of hardware axes to a single logical axis.
/// </summary>
public readonly record struct AxisMapping()
{
    /// <summary>
    ///     Name of axis.
    /// </summary>
    public string AxisName { get; init; } = string.Empty;

    /// <summary>
    ///     List of hardware axes this logical axis is based on.
    /// </summary>
    public ImmutableArray<HardwareAxis> HardwareAxes { get; init; } = ImmutableArray<HardwareAxis>.Empty;
}

/// <summary>
///     Represents a single hardware action defined by a hardware input source i.e. a particular keyboard key press or
///     mouse button press.
/// </summary>
public readonly record struct HardwareAction
{
    /// <summary>
    ///     Hardware input source this hardware action is based on.
    /// </summary>
    public InputSource InputSource { get; init; }
}

/// <summary>
///     Represents a single hardware axis defined by a hardware input source i.e. a particular keyboard key or mouse axis.
/// </summary>
public readonly record struct HardwareAxis
{
    /// <summary>
    ///     Hardware input source this hardware axis is based on.
    /// </summary>
    public InputSource InputSource { get; init; }

    /// <summary>
    ///     Scaling factor of hardware axis state to logical axis state. For discrete input +1 and -1 is mostly used. For
    ///     analog input it can be used as sensitivity.
    /// </summary>
    public double Scale { get; init; }
}