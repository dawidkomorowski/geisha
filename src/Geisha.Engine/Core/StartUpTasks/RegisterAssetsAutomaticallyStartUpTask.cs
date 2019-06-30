﻿using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Configuration;

namespace Geisha.Engine.Core.StartUpTasks
{
    internal class RegisterAssetsAutomaticallyStartUpTask : IStartUpTask
    {
        private readonly IAssetStore _assetStore;
        private readonly IConfigurationManager _configurationManager;

        public RegisterAssetsAutomaticallyStartUpTask(IAssetStore assetStore, IConfigurationManager configurationManager)
        {
            _assetStore = assetStore;
            _configurationManager = configurationManager;
        }

        public void Run()
        {
            var configuration = _configurationManager.GetConfiguration<CoreConfiguration>();
            _assetStore.RegisterAssets(configuration.AssetsRootDirectoryPath);
        }
    }
}