﻿using System.Collections.Generic;

namespace Geisha.Engine.Core.Systems
{
    public interface ISystemsProvider
    {
        IEnumerable<ISystem> GetSystems();
    }
}