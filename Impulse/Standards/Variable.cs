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
            Chars = 3,
            Not_defined = 4
        }

        public string name;
        public variableType type;
        public string value;

        public override string ToString()
        {
            return string.Format(" @{0} ==> {1} : {2}", this.name, this.value, this.type.ToString());
        }
    }
}
