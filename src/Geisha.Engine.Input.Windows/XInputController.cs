using System.Diagnostics;
using SharpDX.XInput;

namespace Geisha.Engine.Input.Windows;

internal static class XInputController
{
    public static void Test()
    {
        var controller = new Controller();
        Debug.WriteLine($"Controller Connected: {controller.IsConnected}");
    }
}