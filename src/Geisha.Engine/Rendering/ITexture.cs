using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Represents texture that is pixel data and attributes of 2D image.
    /// </summary>
    public interface ITexture : IDisposable
    {
        /// <summary>
        ///     Dimensions of texture in pixels.
        /// </summary>
        Vector2 Dimensions { get; }

        /// <summary>
        ///     Unique id of this instance at runtime.
        /// </summary>
        RuntimeId RuntimeId { get; }
    }
}