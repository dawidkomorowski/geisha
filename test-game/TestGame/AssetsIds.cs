using System;
using Geisha.Engine.Core.Assets;

namespace TestGame
{
    public static class AssetsIds
    {
        public static AssetId BoxSprite { get; } = new AssetId(new Guid("72D0650C-996F-4E61-904C-617E940326DE"));
        public static AssetId CompassSprite { get; } = new AssetId(new Guid("09400BA1-A7AB-4752-ADC2-C6535898685C"));

        public static AssetId MusicSound { get; } = new AssetId(new Guid("E23098D1-CE13-4C13-91E0-3CF545EFDFC2"));
        public static AssetId SfxSound { get; } = new AssetId(new Guid("205F7A78-E8FA-49D5-BCF4-3174EBB728FF"));

        public static AssetId PlayerInput { get; } = new AssetId(new Guid("4D5E957B-6176-4FFA-966D-5C3403909D9A"));
    }
}