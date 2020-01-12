using System;
using Geisha.Engine.Core.Assets;

namespace Geisha.IntegrationTestsData
{
    public static class AssetsIds
    {
        public static AssetId TestInputMapping { get; } = new AssetId(new Guid("4DC09263-D7F1-44E2-A9E7-DCA6CCBFE18E"));
        public static AssetId TestSound { get; } = new AssetId(new Guid("94CBB95D-1E7F-4725-912E-0AD90EADACC2"));
        public static AssetId TestTexture { get; } = new AssetId(new Guid("E005E6B6-80E4-49D5-83E6-017C6699CA0E"));
        public static AssetId TestSprite { get; } = new AssetId(new Guid("D249C905-E17D-4BD2-87F6-9DDF80CB7638"));
    }
}