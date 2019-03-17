using System;
using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.IntegrationTests
{
    public static class AssetsIds
    {
        public static AssetId TestInputMapping { get; } = new AssetId(new Guid("4DC09263-D7F1-44E2-A9E7-DCA6CCBFE18E"));
        public static AssetId TestSound { get; } = new AssetId(new Guid("94CBB95D-1E7F-4725-912E-0AD90EADACC2"));
    }
}