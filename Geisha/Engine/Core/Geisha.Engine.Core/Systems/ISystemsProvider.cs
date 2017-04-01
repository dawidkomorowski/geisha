using System.Collections.Generic;

namespace Geisha.Engine.Core.Systems
{
    public interface ISystemsProvider
    {
        IList<ISystem> GetVariableUpdateSystems();
        IList<ISystem> GetFixedUpdateSystems();
    }
}