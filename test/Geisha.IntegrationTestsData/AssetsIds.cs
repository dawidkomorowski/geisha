using System;
using Geisha.Engine.Core.Assets;

namespace Geisha.IntegrationTestsData
{
    public static class AssetsIds
    {
        public static AssetId TestInputMapping { get; } = new AssetId(new Guid("4DC09263-D7F1-44E2-A9E7-DCA6CCBFE18E"));
        public static AssetId TestSound { get; } = new AssetId(new Guid("0d465036-5a99-4267-9401-b636a8c7c0f2"));
        public static AssetId TestTexture { get; } = new AssetId(new Guid("8377E131-DC22-4CF2-AFB5-E7C5A06B3424"));
        public static AssetId TestSprite { get; } = new AssetId(new Guid("999c3421-9c07-423b-aa39-39d736c9da77"));
        public static AssetId TestSpriteAnimation { get; } = new AssetId(new Guid("111DDEEC-E4E1-4447-85CF-39AED5E0B7F0"));
        public static AssetId TestSpriteAnimationFrame1 { get; } = new AssetId(new Guid("7B8D31A4-7A47-4E35-A512-1EBC325CC564"));
        public static AssetId TestSpriteAnimationFrame2 { get; } = new AssetId(new Guid("1387DD0F-849F-4864-9662-9DD26A79EC34"));
        public static AssetId TestSpriteAnimationFrame3 { get; } = new AssetId(new Guid("752DAA7E-41E1-445D-8711-F2B21213B1D9"));

        public static class Sprites
        {
            public static AssetId Sample01 { get; } = new AssetId(new Guid("29af717a-17b7-4f78-9319-43cc1fda62c7"));
        }

        public static class SpriteSheet
        {
            public static AssetId Texture { get; } = new AssetId(new Guid("23c11b21-9acd-457c-9f28-df89db536ed9"));
            public static AssetId FullSprite { get; } = new AssetId(new Guid("2aa860be-5ecb-4609-8466-040adc6ef79c"));
            public static AssetId Part0Sprite { get; } = new AssetId(new Guid("cf4391c4-90cb-45cb-889e-fd4983ca5759"));
            public static AssetId Part1Sprite { get; } = new AssetId(new Guid("b3a40c93-8970-4ab6-96fa-ea27be25d284"));
            public static AssetId Part2Sprite { get; } = new AssetId(new Guid("708233d6-81aa-4a84-ac04-d8c6d393f62a"));
            public static AssetId Part3Sprite { get; } = new AssetId(new Guid("3764f242-0d55-41dd-a5ee-8be2cc5c83da"));
        }
    }
}