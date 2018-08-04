using System;

namespace Geisha.Framework.Rendering
{
    public interface IWindow
    {
        int ClientAreaWidth { get; }
        int ClientAreaHeight { get; }
        IntPtr Handle { get; }
    }
}