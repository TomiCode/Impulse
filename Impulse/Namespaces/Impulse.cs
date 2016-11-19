using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Impulse
{
  class Impulse
  {
    static MethodInfo __localFunction;

    public static object callFunction()
    {
      if(Impulse.__localFunction == null) {
        throw new Exception("Function not requested.");
      }

      if(Impulse.__localFunction.GetParameters().Length > 0) {
        if(!LocalArguments.containsArguments()) {
          throw new Exception("No parameters supplied to this method!");
        }

        try {
          return Impulse.__localFunction.Invoke(null, LocalArguments.getFunctionParameters(Impulse.__localFunction.GetParameters().First().ParameterType));
        }
        catch(Exception e) {
          if(e.InnerException != null) throw e.InnerException;
          else throw e;
        }
      }
      return Impulse.__localFunction.Invoke(null, null);
    }

    public static void requestMethod(string fn)
    {
      Impulse.__localFunction = typeof(Impulse).GetMethod(fn);
      if(Impulse.__localFunction == null) {
        throw new Exception("Function does not exist.");
      }
    }

    public static void println(object[] args)
    {
      foreach(var a in args)
        Console.Write("{0} ", a);
      Console.WriteLine();
    }

    public static int test(int arg1, int arg2)
    {
      Console.WriteLine("Adding arg1 to arg2!");
      return arg1 + arg2;
    }
  }
}
