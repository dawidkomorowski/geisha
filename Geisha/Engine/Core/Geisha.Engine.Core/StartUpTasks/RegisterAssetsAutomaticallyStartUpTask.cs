using Geisha.Engine.Core.Assets;

namespace Geisha.Engine.Core.StartUpTasks
{
    internal interface IRegisterAssetsAutomaticallyStarUpTask
    {
        void Run();
    }

    internal class RegisterAssetsAutomaticallyStartUpTask : IRegisterAssetsAutomaticallyStarUpTask
    {
        private readonly IAssetsDiscoveryEngine _assetsDiscoveryEngine;
        private readonly IAssetStore _assetStore;

        public RegisterAssetsAutomaticallyStartUpTask(IAssetsDiscoveryEngine assetsDiscoveryEngine, IAssetStore assetStore)
        {
            _assetsDiscoveryEngine = assetsDiscoveryEngine;
            _assetStore = assetStore;
        }

        public void Run()
        {
            foreach (var assetInfo in _assetsDiscoveryEngine.DiscoverAssets())
            {
                _assetStore.RegisterAsset(assetInfo);
            }
        }
    }
}