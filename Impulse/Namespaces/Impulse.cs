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

    public static bool hasResult()
    {
      if(Impulse.__localFunction == null)
        return false;

      return (Impulse.__localFunction.ReturnType != typeof(void));
    }

    public static void requestMethod(string fn)
    {
      Impulse.__localFunction = typeof(Impulse).GetMethod(fn);
      if(Impulse.__localFunction == null) {
        throw new Exception("Function does not exist.");
      }
    }

    public static string getFunctionName()
    {
      if(Impulse.__localFunction == null)
        return "<nil>()";

      return Impulse.__localFunction.Name;
    }

    public static void println(IVariable[] args)
    {
      foreach(var a in args) Console.Write("{0} ", a);
      Console.WriteLine();
    }

    public static object read_line()
    {
      Console.Write("> ");
      return Console.ReadLine();
    }

    public static object read_num()
    {
      decimal number;
      Console.Write("> ");
      if(decimal.TryParse(Console.ReadLine(), out number)) {
        return number;
      }

      return default(decimal);
    }

    public static object read_float()
    {
      float number;
      Console.Write("> ");
      if(float.TryParse(Console.ReadLine(), out number)) {
        return number;
      }
      return default(float);
    }

    public static object add(IVariable[] args)
    {
      if(args.Length < 2)
        throw new Exception("Requires min. 2 parameters.");

      Type t = args[0].getType();
      foreach(var arg in args) {
        if(arg.getType() != t) {
          throw new Exception("Argument types differ!");
        }
      }

      if(t == typeof(decimal)) {
        decimal result = args[0].getValue<decimal>();
        for(int i = 1; i < args.Length; i++) {
          result += args[i].getValue<decimal>();
        }
        return result;
      }
      else if(t == typeof(float)) {
        float result = args[0].getValue<float>();
        for(int i = 1; i < args.Length; i++) {
          result += args[i].getValue<float>();
        }
        return result;
      }
      else if(t == typeof(char)) {
        char result = args[0].getValue<char>();
        for(int i = 1; i < args.Length; i++) {
          result += args[i].getValue<char>();
        }
        return result;
      }
      else if(t == typeof(string)) {
        string result = "";
        foreach(IVariable arg in args) {
          result += arg.getValue<string>();
        }
        return result;
      }
      else {
        throw new Exception("Invalid argument types.");
      }
    }

    public static object sub(IVariable[] args)
    {
      if(args.Length < 2)
        throw new Exception("Requires min. 2 parameters.");

      Type t = args[0].getType();
      foreach(var arg in args) {
        if(arg.getType() != t) {
          throw new Exception("Argument types differ!");
        }
      }
      if(t == typeof(decimal)) {
        decimal result = args[0].getValue<decimal>();
        for(int i = 1; i < args.Length; i++) {
          result -= args[i].getValue<decimal>();
        }
        return result;
      }
      else if(t == typeof(float)) {
        float result = args[0].getValue<float>();
        for(int i = 1; i < args.Length; i++) {
          result -= args[i].getValue<float>();
        }
        return result;
      }
      else if(t == typeof(char)) {
        char result = args[0].getValue<char>();
        for(int i = 1; i < args.Length; i++) {
          result -= args[i].getValue<char>();
        }
        return result;
      }
      else {
        throw new Exception("Invalid argument types.");
      }
    }

    public static object mul(IVariable arg1, IVariable arg2)
    {
      if(arg1.getType() != arg2.getType()) {
        throw new Exception("Argument types differ.");
      }
      if(arg1.getType() == typeof(decimal)) {
        return arg1.getValue<decimal>() * arg2.getValue<decimal>();
      }
      else if(arg1.getType() == typeof(float)) {
        return arg1.getValue<float>() * arg2.getValue<float>();
      }
      else {
        throw new Exception("Invalid argument types.");
      }
    }

    public static object div(IVariable arg1, IVariable arg2)
    {
      if(arg1.getType() != arg2.getType()) {
        throw new Exception("Argument types differ.");
      }
      if(arg1.getType() == typeof(decimal)) {
        return arg1.getValue<decimal>() / arg2.getValue<decimal>();
      }
      else if(arg1.getType() == typeof(float)) {
        return arg1.getValue<float>() / arg2.getValue<float>();
      }
      else {
        throw new Exception("Invalid argument types.");
      }
    }

    public static object mod(IVariable arg1, IVariable arg2)
    {
      if(arg1.getType() != arg2.getType()) {
        throw new Exception("Argument types differ.");
      }
      if(arg1.getType() == typeof(decimal)) {
        return arg1.getValue<decimal>() % arg2.getValue<decimal>();
      }
      else if(arg1.getType() == typeof(float)) {
        return arg1.getValue<float>() % arg2.getValue<float>();
      }
      else {
        throw new Exception("Invalid argument types.");
      }
    }
  }
}
