namespace Geisha.Engine.Windowing;

// TODO: Add documentation.
public interface IWindowingSystem
{
    bool CursorVisible { get; set; }
    DisplayMode DisplayMode { get; set; }
}