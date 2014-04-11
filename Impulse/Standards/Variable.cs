using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impulse.Variable;

namespace Impulse
{
    class Variables
    {
        private List<Variable> variables;

        public Variables()
        {
            this.variables = new List<Variable>();
        }

        public Variable getVariable(string name)
        {
            return this.getVariable(name, Variable.variableScope.File);
        }

        public Variable getVariable(string name, Variable.variableScope scope)
        {
            for (int i = 0; i < this.variables.Count; i++)
            {
                if (this.variables[i].name == name && this.variables[i].currentScope == scope)
                {
                    return this.variables[i];
                }
            }
            return null;
        }

        public void addVariable(string name)
        {
            this.addVariable(name, "", Variable.variableType.Not_defined, Variable.variableScope.File);
        }

        public void addVariable(string name, string value, Variable.variableType type)
        {
            this.addVariable(name, value, type, Variable.variableScope.File);
        }

        public void addVariable(string name, string value, Variable.variableType type, Variable.variableScope scope)
        {
            bool isVariableDefined = false;
            for (int i = 0; i < this.variables.Count; i++)
            {
                if (this.variables[i].name == name)
                {
                    isVariableDefined = true;
                    break;
                }
            }

            if (!isVariableDefined)
            {
                this.variables.Add(new Variable(name, type, value, scope));
            }
            else
            {
                Debug.drawDebugLine(debugState.Warning, "Variable {0} has multiple definitions!", name); 
            }
        }

        public bool setVariableValue(string name, string value, Variable.variableType type)
        {
            for (int i = 0; i < this.variables.Count; i++)
            {
                if (this.variables[i].name == name)
                {
                    if (this.variables[i].type == type)
                    {
                        this.variables[i].value = value;
                    }
                    else
                    {
                        Debug.drawDebugLine(debugState.Error, "Variable {0}: type confusion! ({1} to {2})", name, this.variables[i].type.ToString(), type.ToString());
                    }
                    return true;
                }
            }
            return false;
        }

        public void clearLocalVariables(string scopeValue)
        {
            int clearCount = 0;

            for (int i = 0; i < this.variables.Count; i++)
            {
                if (this.variables[i].currentScope == Variable.variableScope.Local && this.variables[i].scopeValue == scopeValue)
                {
                    this.variables.Remove(this.variables[i]);
                    clearCount++;
                }
            }

            Debug.drawDebugLine(debugState.Debug, "Cleared {0} variables (with scopeValue {1})", clearCount, scopeValue);
        }

        public void printAllVariables()
        {
            Console.WriteLine("\n\n== Variables\n");
            for (int i = 0; i < this.variables.Count; i++)
			{
                Console.WriteLine(this.variables[i].ShowInfo());
            }
            Console.WriteLine("\n== End of Variable Table\n");
        }
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
            Local = 2
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

        public string ShowInfo()
        {
            return string.Format(" @{0} => {1} : {2}  [{3} : {4}]", this.name, this.value, this.type.ToString(), this.currentScope.ToString(), this.scopeValue);
        }
    }
}
