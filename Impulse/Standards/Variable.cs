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
}
