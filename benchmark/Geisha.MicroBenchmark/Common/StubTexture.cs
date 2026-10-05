using Geisha.Engine.Core;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;

namespace Geisha.MicroBenchmark.Common;

internal sealed class StubTexture : ITexture
{
    public Vector2 Dimensions { get; } = new(64, 64);
    public RuntimeId RuntimeId { get; } = RuntimeId.Next();

    public void Dispose()
    {
    }
}