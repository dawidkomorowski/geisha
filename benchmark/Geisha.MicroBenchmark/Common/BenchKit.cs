using System;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;

namespace Geisha.MicroBenchmark.Common;

internal static class BenchKit
{
    public static StubTexture Texture { get; } = new();

    public static Sprite CreateSprite()
    {
        return new Sprite(
            Texture,
            new Vector2(0, 0),
            new Vector2(64, 64),
            new Vector2(32, 32),
            1
        );
    }

    public static void ThrowExpectationFailed() => throw new InvalidOperationException("Expectation failed.");
}