using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Impulse
{
  class Variables
  {
    static Hashtable variables = new Hashtable(StringComparer.InvariantCulture);

    public static bool isSet(string variable)
    {
      return Variables.variables.ContainsKey((string)variable);
    }

    public static void setValue(string variable, object content)
    {
      if(!Variables.isSet(variable)) {
        if(content == null) {
          Debug.drawDebugLine(debugState.Debug, "Creating variable '{0}' without value.", variable);
        }
        else {
          Debug.drawDebugLine(debugState.Debug, "Creating variable '{0}' with type {1}.", variable, content.GetType().ToString());
        }
      }
      else {
        if(content == null) {
          Debug.drawDebugLine(debugState.Info, "Set variable '{0}' => nil", variable);
        }
        else {
          Debug.drawDebugLine(debugState.Info, "Set variable '{0}' => {1}", variable, content.GetType().ToString());
        }
      }
      Variables.variables[(string)variable] = content;
    }

    public static object getValue(string variable)
    {
      if(!Variables.isSet(variable)) {
        Debug.drawDebugLine(debugState.Warning, "Accessing undefined variable '{0}'!", variable);
        return null;
      }
      return Variables.variables[(string)variable];
    }

    public static void printContent()
    {
      Console.WriteLine("\n\n--- Variable debug print ---");
      foreach(DictionaryEntry variable in Variables.variables) {
        if(variable.Value == null) {
          Console.WriteLine(" '{0}' => nil", variable.Key);
        }
        else {
          Console.WriteLine(" '{0}' => '{1}' [{2}]", variable.Key, variable.Value, variable.Value.GetType().ToString());
        }
      }
    }
  }

  class LocalArguments
  {
    static List<IVariable> arguments = new List<IVariable>();

    public static object getArgument(int index)
    {
      if(LocalArguments.arguments.Count >= index) {
        Debug.drawDebugLine(debugState.Warning, "Accessing invalid argument at index {0}!", index);
        return null;
      }
      return LocalArguments.arguments[index];
    }

    public static bool isArgumentType(int index, Type type)
    {
      if(index < LocalArguments.arguments.Count) {
        return (LocalArguments.arguments[index].GetType() == type);
      }
      else {
        Debug.drawDebugLine(debugState.Warning, "Accessing invalid argument index '{0}'!", index);
        return false;
      }
    }

    public static object[] getArguments(int index)
    {
      return LocalArguments.arguments.ToArray();
    }

    public static void addArgument(IVariable arg)
    {
      LocalArguments.arguments.Add(arg);
    }

    public static object[] getFunctionParameters(Type param)
    {
      if(param == typeof(IVariable[])) {
        return new object[] { LocalArguments.arguments.ToArray() };
      }
      else {
        return LocalArguments.arguments.ToArray();
      }
    }

    public static void clearArguments()
    {
      Debug.drawDebugLine(debugState.Info, "Clearing localArguments (count: {0}).", LocalArguments.arguments.Count);
      LocalArguments.arguments.Clear();
    }

    public static bool containsArguments()
    {
      return LocalArguments.arguments.Count > 0;
    }
  }

  interface IVariable
  {
    T getValue<T>();
    Type getType();
    bool setValue(object value);
    string ToString();
  }

  class VariableObject : IVariable
  {
    private string name = "";
    private bool create = false;

    public VariableObject(string name, bool create)
    {
      this.name = name;
      this.create = create;

      if(!Variables.isSet(name) && !create) {
        Debug.drawDebugLine(debugState.Warning, "Variable '{0}' does not exists.", name);
      }
    }

    public VariableObject(string name) : this(name, false) { }

    public T getValue<T>()
    {
      if(Variables.isSet(this.name)) {
        object value = Variables.getValue(this.name);
        Debug.drawDebugLine(debugState.Info, "getValue with type {0}, current value: {1}.", typeof(T).ToString(), value);

        if(value != null && value.GetType() == typeof(T)) {
          return (T)value;
        }
      }

      Debug.drawDebugLine(debugState.Warning, "Variable '{0}' is not accesible.", this.name);
      return default(T);
    }

    public Type getType()
    {
      object value = Variables.getValue(this.name);

      if(value != null) {
        return value.GetType();
      }
      else return null;
    }

    public bool setValue(object value)
    {
      if(Variables.isSet(this.name) || this.create) {
        Variables.setValue(this.name, value);
        return true;
      }

      Debug.drawDebugLine(debugState.Error, "Can not write to undefined variable '{0}'!", this.name);
      return false;
    }

    public override string ToString()
    {
      object value = Variables.getValue(this.name);
      if(value == null)
        return "nil";

      return value.ToString();
    }
  }

  class ParameterObject : IVariable
  {
    private object value;

    public ParameterObject(object value)
    {
      this.value = value;
    }

    public T getValue<T>()
    {
      if(value != null) {
        if(value.GetType() == typeof(T)) {
          return (T)value;
        }
      }
      return default(T);
    }

    public Type getType()
    {
      if(this.value == null)
        return null;

      return this.value.GetType();
    }

    public bool setValue(object value)
    {
      return false;
    }

    public override string ToString()
    {
      if(this.value == null)
        return "nil";

      return this.value.ToString();
    }
  }
}
