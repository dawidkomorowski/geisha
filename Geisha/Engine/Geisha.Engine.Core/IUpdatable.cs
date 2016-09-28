using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Engine.Core
{
    public interface IUpdatable
    {
        void Update(double deltaTime);
        void FixedUpdate();
    }
}