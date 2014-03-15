using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impulse
{
    class Impulse
    {
        public virtual bool executeFunctionNamespace(params string[] args)
        {
            Debug.drawDebugLine(debugState.Info, "Call virtual function {1}:{0}", args.Length, args[0]);
            return false;
        }
    }
}
