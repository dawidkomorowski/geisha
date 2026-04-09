using System.Collections.Generic;
using System.Collections.Immutable;

namespace Geisha.Engine.Input.Mapping;

/// <summary>
///     Defines mapping of hardware input to actions and axes.
/// </summary>
public class InputMapping
{
    /// <summary>
    ///     List of action mappings.
    /// </summary>
    public List<ActionMapping> ActionMappings { get; } = new();

    /// <summary>
    ///     List of axis mappings.
    /// </summary>
    public List<AxisMapping> AxisMappings { get; } = new();
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
    public List<HardwareAction> HardwareActions { get; init; } = new();
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
///     Represents a single hardware action that is defined by hardware input variant i.e. a particular keyboard key press.
/// </summary>
public readonly record struct HardwareAction
{
    /// <summary>
    ///     Hardware input variant this hardware action is based on.
    /// </summary>
    public HardwareInputVariant HardwareInputVariant { get; init; }
}

/// <summary>
///     Represents a single hardware axis that is defined by hardware input variant i.e. a particular keyboard key press.
/// </summary>
public readonly record struct HardwareAxis
{
    /// <summary>
    ///     Hardware input variant this hardware axis is based on.
    /// </summary>
    public HardwareInputVariant HardwareInputVariant { get; init; }

    /// <summary>
    ///     Scaling factor of hardware axis state to logical axis state. For discrete input +1 and -1 is mostly used. For
    ///     analog input it can be used as sensitivity.
    /// </summary>
    public double Scale { get; init; }
}