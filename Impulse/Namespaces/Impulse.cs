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
    public static object callStdMethod(string method, Argument[] args)
    {
      MethodInfo functionCaller = Impulse.stdNamespaceInfo(method);
      if(functionCaller == null)
        throw new Exception("Function does not exists.");

      if(functionCaller.GetParameters().Length > 0) {
        if(functionCaller.GetParameters().First().ParameterType == typeof(Argument[])) {
          return functionCaller.Invoke(null, new object[] { args });
        }
        else {
          try {
            return functionCaller.Invoke(null, (object[])args.Cast<object>().ToArray());
          }
          catch (Exception e){
            if(e.InnerException != null) throw e.InnerException;
            else throw e;
          }
        }
      }
      return Impulse.stdNamespaceInfo(method).Invoke(null, null);
    }

    static MethodInfo stdNamespaceInfo(string fn)
    {
      return typeof(Impulse).GetMethod(fn);
    }

    public static void println(Argument[] args)
    {
      foreach(var a in args)
        Console.Write("{0} ", a.value);
      Console.WriteLine();
    }

    public static void test(Argument arg1)
    {
      Console.WriteLine("Argument 1: {0}", arg1.value);
      throw new Exception("Ala ma kota");
    }
  }
}
