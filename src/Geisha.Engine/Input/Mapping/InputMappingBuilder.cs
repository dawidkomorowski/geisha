using System.Collections.Generic;
using System.Collections.Immutable;

namespace Geisha.Engine.Input.Mapping;

public sealed class InputMappingBuilder
{
    private readonly Dictionary<string, ImmutableArray<HardwareAction>.Builder> _actionMappings = new();
    private readonly Dictionary<string, ImmutableArray<HardwareAxis>.Builder> _axisMappings = new();

    public InputMappingBuilder MapAction(string actionName, Key key) => MapAction(actionName, InputSource.Create(key));
    public InputMappingBuilder MapAction(string actionName, MouseButton mouseButton) => MapAction(actionName, InputSource.Create(mouseButton));

    public InputMappingBuilder MapAxis(string axisName, Key key, double scale) => MapAxis(axisName, InputSource.Create(key), scale);
    public InputMappingBuilder MapAxis(string axisName, MouseAxis mouseAxis, double scale) => MapAxis(axisName, InputSource.Create(mouseAxis), scale);

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