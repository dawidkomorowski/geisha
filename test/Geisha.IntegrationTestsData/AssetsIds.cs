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
    }
}