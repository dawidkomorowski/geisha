using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Geisha.Engine.Windowing.Windows;

internal static class NativeCursor
{
    public static void ClipCursorToWindow(Form window)
    {
        var windowScreenRectangle = window.RectangleToScreen(window.ClientRectangle);
        var rect = new Rect
        {
            Left = windowScreenRectangle.Left,
            Top = windowScreenRectangle.Top,
            Right = windowScreenRectangle.Right,
            Bottom = windowScreenRectangle.Bottom
        };
        ClipCursor(rect);
    }

    public static void ReleaseCursorClip()
    {
        ClipCursor(IntPtr.Zero);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ClipCursor(in Rect rect);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ClipCursor(IntPtr rect);
}