using NUnit.Framework;

namespace Geisha.Engine.E2ETests
{
    [Timeout(30_000)]
    public class Tests
    {
        [Test]
        public void EngineApiCanBeInjectedToCustomGameCode()
        {
            var output = E2EApp.Run("EngineApiCanBeInjectedToCustomGameCode");

            // Engine API Injected Into SceneBehavior
            E2EAssert.Reported(output, "3211DA7A-5A4C-409D-B8F6-D82816D7CFA2",
                "Engine API Injected Into SceneBehavior: Geisha.Engine.Audio.NAudio.NAudioAudioBackend");
            E2EAssert.Reported(output, "C7897578-6670-4DEA-A32F-689629FE651E",
                "Engine API Injected Into SceneBehavior: Geisha.Engine.Core.EngineManager");
            E2EAssert.Reported(output, "B94536CE-5369-4105-B901-EC878E20E71F",
                "Engine API Injected Into SceneBehavior: Geisha.Engine.Core.Assets.AssetStore");
            E2EAssert.Reported(output, "2F59C6C4-5183-4B33-9433-9AD9995F0923",
                "Engine API Injected Into SceneBehavior: Geisha.Engine.Rendering.DebugRenderer");
            E2EAssert.Reported(output, "A3DB27E0-2A71-4728-9BE8-5C060F406EC8",
                "Engine API Injected Into SceneBehavior: Geisha.Engine.Core.SceneModel.SceneLoader");
            E2EAssert.Reported(output, "5C9C1856-8DF1-4E2D-BEF3-BB524FC62544",
                "Engine API Injected Into SceneBehavior: Geisha.Engine.Core.SceneModel.SceneManager");
            E2EAssert.Reported(output, "56048FE9-5C59-44F5-8C4C-D96615B62D8C",
                "Engine API Injected Into SceneBehavior: Geisha.Engine.Core.SceneModel.Serialization.SceneSerializer");
            E2EAssert.Reported(output, "8DC6B886-CC5C-431E-821A-900D3671CB70",
                "Engine API Injected Into SceneBehavior: Geisha.Engine.Core.Coroutines.CoroutineSystem");
            E2EAssert.Reported(output, "59797ECF-0B77-4CA2-B389-020B884B9E8F",
                "Engine API Injected Into SceneBehavior: Geisha.Engine.Physics.Systems.PhysicsSystem");

            // Engine API Injected Into Component
            E2EAssert.Reported(output, "484E1AFA-EEFE-4E3A-9D8E-A304847C8C16",
                "Engine API Injected Into Component: Geisha.Engine.Audio.NAudio.NAudioAudioBackend");
            E2EAssert.Reported(output, "568407AA-0471-42BD-8CBD-6CB2A7526B76",
                "Engine API Injected Into Component: Geisha.Engine.Core.EngineManager");
            E2EAssert.Reported(output, "7B72B6EB-69BC-49F2-BEA5-CC073581F1D0",
                "Engine API Injected Into Component: Geisha.Engine.Core.Assets.AssetStore");
            E2EAssert.Reported(output, "4449A465-20AB-4E99-9C62-EB475387910D",
                "Engine API Injected Into Component: Geisha.Engine.Rendering.DebugRenderer");
            E2EAssert.Reported(output, "462F0430-A3D3-4E2D-91C6-A4C8EBBE24C8",
                "Engine API Injected Into Component: Geisha.Engine.Core.SceneModel.SceneLoader");
            E2EAssert.Reported(output, "035F113D-43D8-4B92-B4DF-D1F6FDCBEEC9",
                "Engine API Injected Into Component: Geisha.Engine.Core.SceneModel.SceneManager");
            E2EAssert.Reported(output, "899851F9-822D-4826-860B-7AB4C611DAC1",
                "Engine API Injected Into Component: Geisha.Engine.Core.SceneModel.Serialization.SceneSerializer");
            E2EAssert.Reported(output, "4AF66A42-E328-471B-BDB0-C0D987F5EAE1",
                "Engine API Injected Into Component: Geisha.Engine.Core.Coroutines.CoroutineSystem");
            E2EAssert.Reported(output, "11519AAE-5E1A-4462-973A-81B09672721D",
                "Engine API Injected Into Component: Geisha.Engine.Physics.Systems.PhysicsSystem");

            // Engine API Injected Into System
            E2EAssert.Reported(output, "E7691D98-AF87-4268-9C39-43822A790377",
                "Engine API Injected Into System: Geisha.Engine.Audio.NAudio.NAudioAudioBackend");
            E2EAssert.Reported(output, "FE445F35-E624-4BCF-800C-FAD91F3C0216",
                "Engine API Injected Into System: Geisha.Engine.Core.EngineManager");
            E2EAssert.Reported(output, "A9236158-3810-41A0-B14B-8516A39E404B",
                "Engine API Injected Into System: Geisha.Engine.Core.Assets.AssetStore");
            E2EAssert.Reported(output, "9F0D1065-B9AF-4DD6-9ED7-06D3462E5795",
                "Engine API Injected Into System: Geisha.Engine.Rendering.DebugRenderer");
            E2EAssert.Reported(output, "FF5B94ED-12B5-43E6-9F3F-0A57BEB162BA",
                "Engine API Injected Into System: Geisha.Engine.Core.SceneModel.SceneLoader");
            E2EAssert.Reported(output, "D89648A6-5676-492E-ADB8-6296C1B8BEE6",
                "Engine API Injected Into System: Geisha.Engine.Core.SceneModel.SceneManager");
            E2EAssert.Reported(output, "DD6882F2-4A1B-42F3-993C-473593C46DE5",
                "Engine API Injected Into System: Geisha.Engine.Core.SceneModel.Serialization.SceneSerializer");
            E2EAssert.Reported(output, "618427C0-D078-451C-8877-B3B81C99B5FF",
                "Engine API Injected Into System: Geisha.Engine.Core.Coroutines.CoroutineSystem");
            E2EAssert.Reported(output, "932A0F77-F5F9-4CB5-B3EE-56FDF6139291",
                "Engine API Injected Into System: Geisha.Engine.Physics.Systems.PhysicsSystem");
        }
    }
}