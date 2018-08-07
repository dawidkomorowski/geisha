using System;
using Geisha.Common.Math;

namespace Geisha.Framework.Rendering
{
    public interface ITexture : IDisposable
    {
        Vector2 Dimension { get; }
    }
}