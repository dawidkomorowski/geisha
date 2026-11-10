using System;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Input.Backend;
using SharpDX.XInput;
using Vibration = Geisha.Engine.Input.Backend.Vibration;

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

        var leftTriggerNormalized = NormalizeXInputTrigger(xInputState.Gamepad.LeftTrigger);
        var rightTriggerNormalized = NormalizeXInputTrigger(xInputState.Gamepad.RightTrigger);

        var leftThumbXNormalized = NormalizeXInputThumb(xInputState.Gamepad.LeftThumbX);
        var leftThumbYNormalized = NormalizeXInputThumb(xInputState.Gamepad.LeftThumbY);

        var rightThumbXNormalized = NormalizeXInputThumb(xInputState.Gamepad.RightThumbX);
        var rightThumbYNormalized = NormalizeXInputThumb(xInputState.Gamepad.RightThumbY);

        return new ControllerState
        {
            Index = controllerIndex,
            IsConnected = isConnected,
            PacketNumber = xInputState.PacketNumber,
            Buttons = (ControllerButtons)xInputState.Gamepad.Buttons,
            LeftTrigger = leftTriggerNormalized,
            RightTrigger = rightTriggerNormalized,
            LeftThumb = new Vector2(leftThumbXNormalized, leftThumbYNormalized),
            RightThumb = new Vector2(rightThumbXNormalized, rightThumbYNormalized)
        };
    }

    public static void SetVibration(ControllerIndex controllerIndex, Vibration vibration)
    {
        var controller = GetController(controllerIndex);
        controller.SetVibration(new SharpDX.XInput.Vibration
        {
            LeftMotorSpeed = ToXInputMotorSpeed(vibration.LeftMotorSpeed),
            RightMotorSpeed = ToXInputMotorSpeed(vibration.RightMotorSpeed)
        });
    }

    private static double NormalizeXInputTrigger(byte value) => (double)value / byte.MaxValue;

    private static double NormalizeXInputThumb(short value) => value >= 0 ? (double)value / short.MaxValue : (double)value / -short.MinValue;

    private static ushort ToXInputMotorSpeed(double value) => (ushort)(Math.Clamp(value, 0d, 1d) * ushort.MaxValue);
}