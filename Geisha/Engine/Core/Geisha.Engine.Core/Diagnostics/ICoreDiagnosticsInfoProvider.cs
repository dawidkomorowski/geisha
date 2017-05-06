using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Diagnostics
{
    public interface ICoreDiagnosticsInfoProvider
    {
        void UpdateDiagnostics(Scene scene);
    }
}