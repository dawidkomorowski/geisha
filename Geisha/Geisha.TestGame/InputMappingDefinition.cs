using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;

namespace Geisha.TestGame
{
    internal static class InputMappingDefinition
    {
        public static InputMapping BoxInputMapping
        {
            get
            {
                var inputMapping = new InputMapping();

                // Action mappings
                var jetRotateRight = new ActionMappingGroup {ActionName = "JetRotateRight"};
                var jetRotateRightActionMapping = new ActionMapping
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Space)
                };
                jetRotateRight.ActionMappings.Add(jetRotateRightActionMapping);
                inputMapping.ActionMappingGroups.Add(jetRotateRight);

                // Axis mappings
                var moveUp = new AxisMappingGroup {AxisName = "MoveUp"};
                moveUp.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Up),
                    Scale = 1
                });
                moveUp.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Down),
                    Scale = -1
                });
                inputMapping.AxisMappingGroups.Add(moveUp);

                var moveRight = new AxisMappingGroup {AxisName = "MoveRight"};
                moveRight.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Right),
                    Scale = 1
                });
                moveRight.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Left),
                    Scale = -1
                });
                inputMapping.AxisMappingGroups.Add(moveRight);

                return inputMapping;
            }
        }
    }
}