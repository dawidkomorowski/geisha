using System;
using Geisha.Engine.Input.Backend;
using SharpDX.XInput;

namespace Geisha.Engine.Input.Windows;

internal static class XInputController
{
    private static readonly Controller Controller1 = new(UserIndex.One);
    private static readonly Controller Controller2 = new(UserIndex.Two);
    private static readonly Controller Controller3 = new(UserIndex.Three);
    private static readonly Controller Controller4 = new(UserIndex.Four);

    private static Controller GetController(ControllerIndex controllerIndex) => controllerIndex switch
    {
        ControllerIndex.One => Controller1,
        ControllerIndex.Two => Controller2,
        ControllerIndex.Three => Controller3,
        ControllerIndex.Four => Controller4,
        _ => throw new ArgumentOutOfRangeException(nameof(controllerIndex), controllerIndex, null)
    };

    public static ControllerState GetState(ControllerIndex controllerIndex)
    {
        var controller = GetController(controllerIndex);
        var isConnected = controller.GetState(out var xInputState);

        return new ControllerState
        {
            Index = controllerIndex,
            IsConnected = isConnected,
            PacketNumber = xInputState.PacketNumber
        };
    }
}