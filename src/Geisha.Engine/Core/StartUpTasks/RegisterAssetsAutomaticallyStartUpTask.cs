using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Core.StartUpTasks
{
    internal class RegisterAssetsAutomaticallyStartUpTask : IStartUpTask
    {
        private readonly IAssetStore _assetStore;
        private readonly CoreConfiguration _configuration;

        public RegisterAssetsAutomaticallyStartUpTask(IAssetStore assetStore, CoreConfiguration configuration)
        {
            _assetStore = assetStore;
            _configuration = configuration;
        }

        public void Run()
        {
            _assetStore.RegisterAssets(_configuration.AssetsRootDirectoryPath);
        }
    }
}