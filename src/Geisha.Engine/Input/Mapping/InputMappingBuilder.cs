using System.Collections.Generic;
using System.Collections.Immutable;

namespace Geisha.Engine.Input.Mapping;

/// <summary>
///     Builds <see cref="InputMapping" /> by mapping hardware input sources to logical actions and axes.
/// </summary>
/// <remarks>
///     Mapping multiple hardware input sources to the same action or axis name adds all sources to that logical mapping.
/// </remarks>
public sealed class InputMappingBuilder
{
    private readonly Dictionary<string, ImmutableArray<HardwareAction>.Builder> _actionMappings = new();
    private readonly Dictionary<string, ImmutableArray<HardwareAxis>.Builder> _axisMappings = new();

    /// <summary>
    ///     Maps specified keyboard key to logical action.
    /// </summary>
    /// <param name="actionName">Name of logical action.</param>
    /// <param name="key">Keyboard key that triggers the action.</param>
    /// <returns>This <see cref="InputMappingBuilder" /> instance.</returns>
    public InputMappingBuilder MapAction(string actionName, Key key) => MapAction(actionName, InputSource.Create(key));

    /// <summary>
    ///     Maps specified mouse button to logical action.
    /// </summary>
    /// <param name="actionName">Name of logical action.</param>
    /// <param name="mouseButton">Mouse button that triggers the action.</param>
    /// <returns>This <see cref="InputMappingBuilder" /> instance.</returns>
    public InputMappingBuilder MapAction(string actionName, MouseButton mouseButton) => MapAction(actionName, InputSource.Create(mouseButton));

    /// <summary>
    ///     Maps specified keyboard key to logical axis with given scale.
    /// </summary>
    /// <param name="axisName">Name of logical axis.</param>
    /// <param name="key">Keyboard key that contributes to the axis state.</param>
    /// <param name="scale">Scale applied to the key state when computing axis value.</param>
    /// <returns>This <see cref="InputMappingBuilder" /> instance.</returns>
    public InputMappingBuilder MapAxis(string axisName, Key key, double scale) => MapAxis(axisName, InputSource.Create(key), scale);

    /// <summary>
    ///     Maps specified mouse axis to logical axis with given scale.
    /// </summary>
    /// <param name="axisName">Name of logical axis.</param>
    /// <param name="mouseAxis">Mouse axis that contributes to the axis state.</param>
    /// <param name="scale">Scale applied to the mouse axis value when computing axis value.</param>
    /// <returns>This <see cref="InputMappingBuilder" /> instance.</returns>
    public InputMappingBuilder MapAxis(string axisName, MouseAxis mouseAxis, double scale) => MapAxis(axisName, InputSource.Create(mouseAxis), scale);

    /// <summary>
    ///     Builds <see cref="InputMapping" /> from mappings defined in this builder.
    /// </summary>
    /// <returns><see cref="InputMapping" /> containing action mappings and axis mappings defined in this builder.</returns>
    public InputMapping Build()
    {
        var actionMappingsBuilder = ImmutableArray.CreateBuilder<ActionMapping>();

        foreach (var (actionName, builder) in _actionMappings)
        {
            actionMappingsBuilder.Add(new ActionMapping
            {
                ActionName = actionName,
                HardwareActions = builder.ToImmutable()
            });
        }

        var axisMappingsBuilder = ImmutableArray.CreateBuilder<AxisMapping>();

        foreach (var (axisName, builder) in _axisMappings)
        {
            axisMappingsBuilder.Add(new AxisMapping
            {
                AxisName = axisName,
                HardwareAxes = builder.ToImmutable()
            });
        }

        return new InputMapping
        {
            ActionMappings = actionMappingsBuilder.ToImmutable(),
            AxisMappings = axisMappingsBuilder.ToImmutable()
        };
    }

    private InputMappingBuilder MapAction(string actionName, InputSource inputSource)
    {
        if (!_actionMappings.TryGetValue(actionName, out var builder))
        {
            builder = ImmutableArray.CreateBuilder<HardwareAction>();
            _actionMappings.Add(actionName, builder);
        }

        builder.Add(new HardwareAction { InputSource = inputSource });

        return this;
    }

    private InputMappingBuilder MapAxis(string axisName, InputSource inputSource, double scale)
    {
        if (!_axisMappings.TryGetValue(axisName, out var builder))
        {
            builder = ImmutableArray.CreateBuilder<HardwareAxis>();
            _axisMappings.Add(axisName, builder);
        }

        builder.Add(new HardwareAxis { InputSource = inputSource, Scale = scale });

        return this;
    }
}