using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Core.UnitTests.Assets
{
    public interface IManagedAssetSubstitute : IManagedAsset
    {
        bool LoadWasCalled { get; }
        bool UnloadWasCalled { get; }

        void ClearCalledFlags();
    }

    public static class ManagedAssetSubstitute
    {
        public static IManagedAssetSubstitute Create<TAsset>(AssetInfo assetInfo, TAsset asset) where TAsset : class
        {
            return new ManagedAssetSubstituteImpl<TAsset>(assetInfo, asset);
        }

        private sealed class ManagedAssetSubstituteImpl<TAsset> : ManagedAsset<TAsset>, IManagedAssetSubstitute where TAsset : class
        {
            private readonly TAsset _asset;

            public ManagedAssetSubstituteImpl(AssetInfo assetInfo, TAsset asset) : base(assetInfo)
            {
                _asset = asset;
            }

            public bool LoadWasCalled { get; private set; }
            public bool UnloadWasCalled { get; private set; }

            public void ClearCalledFlags()
            {
                LoadWasCalled = false;
                UnloadWasCalled = false;
            }

            protected override TAsset LoadAsset()
            {
                LoadWasCalled = true;
                return _asset;
            }

            protected override void UnloadAsset(TAsset asset)
            {
                UnloadWasCalled = true;
            }
        }
    }
}