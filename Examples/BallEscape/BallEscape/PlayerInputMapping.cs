using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;

namespace BallEscape
{
    public static class PlayerInputMapping
    {
        public static InputMapping CreateInputMapping()
        {
            var inputMapping = new InputMapping();

            var moveUp = new AxisMappingGroup
            {
                AxisName = "MoveUp",
                AxisMappings =
                {
                    new AxisMapping
                    {
                        HardwareInputVariant = new HardwareInputVariant {Key = Key.Up},
                        Scale = 1
                    },
                    new AxisMapping
                    {
                        HardwareInputVariant = new HardwareInputVariant {Key = Key.Down},
                        Scale = -1
                    }
                }
            };

            var moveRight = new AxisMappingGroup
            {
                AxisName = "MoveRight",
                AxisMappings =
                {
                    new AxisMapping
                    {
                        HardwareInputVariant = new HardwareInputVariant {Key = Key.Right},
                        Scale = 1
                    },
                    new AxisMapping
                    {
                        HardwareInputVariant = new HardwareInputVariant {Key = Key.Left},
                        Scale = -1
                    }
                }
            };

            inputMapping.AxisMappingGroups.Add(moveUp);
            inputMapping.AxisMappingGroups.Add(moveRight);

            return inputMapping;
        }
    }
}