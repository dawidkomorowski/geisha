using System.Diagnostics;
using SharpDX.XInput;

namespace Geisha.Engine.Input.Windows;

internal static class XInputController
{
    public static void Test()
    {
        var controller = new Controller(UserIndex.One);
        var isConnected = controller.GetState(out var state);
        Debug.WriteLine($"Controller Connected: {isConnected}, Packet Number: {state.PacketNumber}, Button A: {state.Gamepad.Buttons & GamepadButtonFlags.A}");
    }
}