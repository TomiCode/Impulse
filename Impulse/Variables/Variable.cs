using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impulse
{
    class Variable
    {
        public enum variableType
        {
            String = 0,
            Decimal = 1,
            Float = 2,
            Chars = 3
        }

        public variableType type;
        public string value;
    }
}
