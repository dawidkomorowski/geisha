using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Windowing.Backend;

// TODO: Add documentation.
public interface IWindowingBackend
{
    bool AllowWindowResizing { get; set; }
    Size WindowClientSize { get; set; }
}