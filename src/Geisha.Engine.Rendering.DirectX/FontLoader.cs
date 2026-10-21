using System;
using System.IO;
using System.Runtime.InteropServices;
using NLog;

namespace Geisha.Engine.Rendering.DirectX;

internal static class FontLoader
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private const uint FR_PRIVATE = 0x10;
    private const uint FR_NOT_ENUM = 0x20;

    [DllImport("gdi32.dll", EntryPoint = "AddFontResourceExW", CharSet = CharSet.Unicode)]
    private static extern int AddFontResourceExW(string name, uint fl, IntPtr res);

    [DllImport("gdi32.dll", EntryPoint = "RemoveFontResourceExW", CharSet = CharSet.Unicode)]
    private static extern bool RemoveFontResourceExW(string name, uint fl, IntPtr pdv);

    public static void LoadCustomFont(string fontFilePath)
    {
        if (!File.Exists(fontFilePath))
        {
            throw new FileNotFoundException("Font file not found.", fontFilePath);
        }

        var fontsAdded = AddFontResourceExW(fontFilePath, FR_PRIVATE | FR_NOT_ENUM, IntPtr.Zero);
        if (fontsAdded == 0)
        {
            throw new InvalidOperationException($"Failed to load font from path: {fontFilePath}. The file may be corrupted, locked, or an unsupported format.");
        }
    }

    public static void UnloadCustomFont(string fontFilePath)
    {
        if (string.IsNullOrEmpty(fontFilePath))
        {
            throw new ArgumentException("Font file path must be non empty.", nameof(fontFilePath));
        }

        var success = RemoveFontResourceExW(fontFilePath, FR_PRIVATE | FR_NOT_ENUM, IntPtr.Zero);
        if (!success)
        {
            Logger.Warn("RemoveFontResourceEx returned false for {fontFilePath}. It will be cleaned up automatically by the OS.", fontFilePath);
        }
    }
}