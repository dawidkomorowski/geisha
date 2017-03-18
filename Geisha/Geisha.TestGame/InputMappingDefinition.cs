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
                jetRotateRight.ActionMappings.Add(new ActionMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Space}
                });
                inputMapping.ActionMappingGroups.Add(jetRotateRight);

                // Axis mappings
                var moveUp = new AxisMappingGroup {AxisName = "MoveUp"};
                moveUp.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Up},
                    Scale = 1
                });
                moveUp.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Down},
                    Scale = -1
                });
                inputMapping.AxisMappingGroups.Add(moveUp);

                var moveRight = new AxisMappingGroup {AxisName = "MoveRight"};
                moveRight.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Right},
                    Scale = 1
                });
                moveRight.AxisMappings.Add(new AxisMapping
                {
                    HardwareInputVariant = new HardwareInputVariant {Key = Key.Left},
                    Scale = -1
                });
                inputMapping.AxisMappingGroups.Add(moveRight);

                return inputMapping;
            }
        }
    }
}