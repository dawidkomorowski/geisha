using System;
using Geisha.Common.Math;

namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Represents texture that is pixel data and attributes of 2D image.
    /// </summary>
    public interface ITexture : IDisposable
    {
        /// <summary>
        ///     Dimension of texture in pixels.
        /// </summary>
        Vector2 Dimension { get; }
    }
}