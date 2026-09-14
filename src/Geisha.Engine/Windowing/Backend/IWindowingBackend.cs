using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Windowing.Backend;

// TODO: Add documentation.
// TODO: Allow to hide/show cursor.
public interface IWindowingBackend
{
    bool AllowWindowResizing { get; set; }
    Size WindowClientSize { get; set; }
    DisplayMode DisplayMode { get; set; }
}