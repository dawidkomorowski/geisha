using System.Diagnostics;
using SharpDX.XInput;

namespace Geisha.Engine.Input.Windows;

internal static class XInputController
{
    public static void Test()
    {
        var controller = new Controller(UserIndex.One);
        var state = controller.GetState();
        Debug.WriteLine($"Controller Connected: {controller.IsConnected}");
        Debug.WriteLine($"Packet Number: {state.PacketNumber}");
        Debug.WriteLine($"Button A: {state.Gamepad.Buttons & GamepadButtonFlags.A}");
    }
}