using SharpDX.XInput;

namespace Geisha.Engine.Input.Windows;

internal static class XInputController
{
    public static void Test()
    {
        var controller = new Controller(UserIndex.One);
        var isConnected = controller.GetState(out var state);
    }
}