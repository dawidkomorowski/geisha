using System;
using System.Diagnostics;
using Geisha.Engine.Core.Math;
using SharpDX;
using SharpDX.XInput;

namespace Geisha.Engine.Input.Windows;

internal static class XInputController
{
    private sealed class ControllerContext
    {
        private static readonly TimeSpan ReconnectInterval = TimeSpan.FromSeconds(2);
        private readonly Stopwatch _reconnectTimer;

        public ControllerContext(UserIndex userIndex)
        {
            Controller = new Controller(userIndex);
            IsConnected = true; // Initially true to poll the very first state before waiting for reconnect.
            _reconnectTimer = Stopwatch.StartNew();
        }

        public Controller Controller { get; }
        public bool IsConnected { get; private set; }

        public bool WaitingForReconnect => !IsConnected && _reconnectTimer.Elapsed < ReconnectInterval;

        public void UpdateConnectionStatus(bool isConnected)
        {
            IsConnected = isConnected;
            if (!IsConnected)
            {
                _reconnectTimer.Restart();
            }
        }
    }


    private static readonly ControllerContext Controller1 = new(UserIndex.One);
    private static readonly ControllerContext Controller2 = new(UserIndex.Two);
    private static readonly ControllerContext Controller3 = new(UserIndex.Three);
    private static readonly ControllerContext Controller4 = new(UserIndex.Four);

    private static ControllerContext GetControllerContext(ControllerIndex controllerIndex) => controllerIndex switch
    {
        ControllerIndex.One => Controller1,
        ControllerIndex.Two => Controller2,
        ControllerIndex.Three => Controller3,
        ControllerIndex.Four => Controller4,
        _ => throw new ArgumentOutOfRangeException(nameof(controllerIndex), controllerIndex, null)
    };

    public static ControllerState GetState(ControllerIndex controllerIndex)
    {
        var controllerContext = GetControllerContext(controllerIndex);

        if (controllerContext.WaitingForReconnect)
        {
            return new ControllerState
            {
                Index = controllerIndex,
                IsConnected = false
            };
        }

        var isConnected = controllerContext.Controller.GetState(out var xInputState);
        controllerContext.UpdateConnectionStatus(isConnected);

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
        var controllerContext = GetControllerContext(controllerIndex);

        if (controllerContext.WaitingForReconnect)
        {
            return;
        }

        try
        {
            controllerContext.Controller.SetVibration(new SharpDX.XInput.Vibration
            {
                LeftMotorSpeed = ToXInputMotorSpeed(vibration.LeftMotorSpeed),
                RightMotorSpeed = ToXInputMotorSpeed(vibration.RightMotorSpeed)
            });
            controllerContext.UpdateConnectionStatus(true);
        }
        catch (SharpDXException e)
        {
            controllerContext.UpdateConnectionStatus(false);

            // Workaround for SharpDX: ignore exception thrown when device is disconnected.
            // When migrated to Silk.NET the return value will indicate failure without exception.
            const int deviceDisconnected = unchecked((int)0x8007048F);
            if (e.ResultCode.Code != deviceDisconnected)
            {
                throw;
            }
        }
    }

    private static double NormalizeXInputTrigger(byte value) => (double)value / byte.MaxValue;

    private static double NormalizeXInputThumb(short value) => value >= 0 ? (double)value / short.MaxValue : (double)value / -short.MinValue;

    private static ushort ToXInputMotorSpeed(double value) => (ushort)(Math.Clamp(value, 0d, 1d) * ushort.MaxValue);
}