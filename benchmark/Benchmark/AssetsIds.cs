using System;
using Geisha.Engine.Core.Assets;

namespace Benchmark
{
    internal static class AssetsIds
    {
        public static AssetId PaintColorPalette { get; } = new AssetId(new Guid("23fbeb5c-c61e-4d79-a2df-798389867b25"));
        public static AssetId ExplosionAnimation { get; } = new AssetId(new Guid("73d21937-4e76-4f8e-9376-af7ae090d0ee"));
    }
}