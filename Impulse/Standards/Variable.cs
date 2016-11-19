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
    static Hashtable variables = new Hashtable();

    public static bool isSet(string variable)
    {
      return Variables.variables.ContainsKey(variables);
    }

    public static void setValue(string variable, object content)
    {
      if(!Variables.isSet(variable)) {
        Debug.drawDebugLine(debugState.Debug, "Creating variable '{0}' with type {1}.", variable, content.GetType().ToString());
      }
      else {
        Debug.drawDebugLine(debugState.Info, "Variable {0} maps type {1}. New type: {2}.", variable, 
          Variables.variables[variable].GetType().ToString(), content.GetType().ToString());
      }
      Variables.variables.Add(variable, content);
    }

    public static object getValue(string variable)
    {
      if(!Variables.isSet(variable)) {
        Debug.drawDebugLine(debugState.Warning, "Accessing undefined variable '{0}'!", variable);
        return null;
      }
      return Variables.variables[variable];
    }

    public static void printContent()
    {
      Console.WriteLine("\n\n--- Variable debug print ---");
      foreach(DictionaryEntry variable in Variables.variables) {
        Console.WriteLine(" '{0}' => '{1}' [{2}]", variable.Key, variable.Value, variable.Value.GetType().ToString());
      }
    }
  }

  class LocalArguments
  {
    static List<object> arguments = new List<object>();

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

    public static void addArgument(object arg)
    {
      LocalArguments.arguments.Add(arg);
    }

    public static object[] getFunctionParameters(Type param)
    {
      if(param == typeof(object[])) {
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
  }

  class VariableObject
  {
    private string name = "";

    VariableObject(string name)
    {
      if(!Variables.isSet(name)) {
        Debug.drawDebugLine(debugState.Warning, "Variable '{0}' does not exists.", name);
      }
      this.name = name;
    }

    public object getValue()
    {
      if(Variables.isSet(this.name)) {
        Debug.drawDebugLine(debugState.Warning, "Variable '{0}' is not accesible.", this.name);
        return null;
      }
      return Variables.getValue(this.name);
    }

    public bool setValue(object value)
    {
      if(!Variables.isSet(this.name)) {
        Debug.drawDebugLine(debugState.Error, "Can not write to undefined variable '{0}'!", this.name);
        return false;
      }
      Variables.setValue(this.name, value);
      return true;
    }
  }
}
