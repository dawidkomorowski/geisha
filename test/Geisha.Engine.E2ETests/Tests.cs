using NUnit.Framework;

namespace Geisha.Engine.E2ETests
{
    [Timeout(10_000)]
    public class Tests
    {
        [Test]
        public void EngineApiCanBeInjectedToCustomGameCode()
        {
            var output = E2EApp.Run("EngineApiCanBeInjectedToCustomGameCode");

            E2EAssert.Reported(output, "9CA85BC0-A6B3-44ED-9FA7-C64F0909F1A3", "Engine API Injected Into SceneBehavior");
            E2EAssert.Reported(output, "484E1AFA-EEFE-4E3A-9D8E-A304847C8C16", "Engine API Injected Into Component");
            E2EAssert.Reported(output, "E7691D98-AF87-4268-9C39-43822A790377", "Engine API Injected Into System");
        }
    }
}