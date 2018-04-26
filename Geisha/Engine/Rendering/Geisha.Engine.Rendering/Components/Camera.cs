using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Rendering.Components
{
    // TODO what if there are more than one camera? (introduce active flag?)
    // TODO optimization(camera viewing space is good point to optimizing draw calls by clipping things out of camera or not visible by camera)
    // TODO viewing space:
    // TODO     for 2D it is logical rectangle that is observable clipping of the whole scene
    // TODO     for 3D it is frustum space that defines observable clipping polyhedron
    // TODO resolution(is it camera responsibility?)
    // TODO aspect ratio(isn't it defined by viewing space?)
    // TODO projection type (only meaningful for 3D ?)
    /// <summary>
    ///     Represents camera that defines view-port.
    /// </summary>
    public sealed class Camera : IComponent
    {
    }
}