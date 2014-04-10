using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impulse
{
    class Variables
    {
        private List<Variable> variables;

    }

    class Variable
    {
        public Variable(string name, variableType type, variableScope scope)
        {
            this.name = name;
            this.type = type;
            this.myScope = new varScope(scope);
            this.value = null;
        }

        public Variable(string name, variableType type, variableScope scope, string scopeValue)
        {
            this.name = name;
            this.type = type;
            this.myScope = new varScope(scope, scopeValue);
            this.value = null;
        }

        public Variable(string name, variableType type, string value, variableScope scope)
        {
            this.name = name;
            this.type = type;
            this.value = value;
            this.myScope = new varScope(scope);
        }

        public Variable(string name, variableType type, string value, variableScope scope, string scopeValue)
        {
            this.name = name;
            this.type = type;
            this.value = value;
            this.myScope = new varScope(scope, scopeValue);
        }

        public struct varScope
        {
            public string value;
            public variableScope scope;

            public varScope(variableScope scope)
            {
                this.value = null;
                this.scope = scope;
            }

            public varScope(variableScope scope, string value)
            {
                this.value = value;
                this.scope = scope;
            }
        }

        public enum variableType
        {
            String = 0,
            Decimal = 1,
            Float = 2,
            Chars = 3,
            Not_defined = 4
        }

        public enum variableScope
        {
            Global = 0,
            File = 1,
            Method = 2,
            Local = 3
        }

        public string name;
        public variableType type;
        varScope myScope;
        public string value;

        public variableScope currentScope
        {
            get { return this.myScope.scope; }
        }

        public string scopeValue
        {
            get { return this.myScope.value; }
        }

        public override string ToString()
        {
            return string.Format(" @{0} ==> {1} : {2}", this.name, this.value, this.type.ToString());
        }
    }
}
