using System;
using Geisha.Engine.Core.Assets;

namespace Sandbox
{
    public static class AssetsIds
    {
        public static AssetId BoxSprite { get; } = new AssetId(new Guid("56f5c040-dc60-46c7-b0ed-0353488d85f8"));
        public static AssetId CompassSprite { get; } = new AssetId(new Guid("df819387-5162-4ce5-85be-68aa35ad0faf"));

        public static AssetId CampfireAnimation { get; } = new AssetId(new Guid("72B4483A-4E0E-45B9-BDD0-C4A265976CD8"));

        public static AssetId MusicSound { get; } = new AssetId(new Guid("4f7b501d-2fd7-47f9-888a-5bc514a6ed9a"));
        public static AssetId SfxSound { get; } = new AssetId(new Guid("ab1de87e-cad4-4ab8-8982-b33560faa76d"));

        public static AssetId PlayerInput { get; } = new AssetId(new Guid("4D5E957B-6176-4FFA-966D-5C3403909D9A"));
    }
}