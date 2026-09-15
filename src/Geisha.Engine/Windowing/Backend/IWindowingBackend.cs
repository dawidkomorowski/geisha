using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Windowing.Backend;

// TODO: Add documentation.
// TODO: Allow to hide/show cursor.
public interface IWindowingBackend
{
    string WindowTitle { get; set; }
    Size WindowClientSize { get; set; }
    DisplayMode DisplayMode { get; set; }
    bool AllowWindowResizing { get; set; }
}