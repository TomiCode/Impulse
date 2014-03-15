using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Impulse
{

    public enum debugState
    {
        Info = 0,
        Warning = 1,
        Error = 2,
        Debug = 3,
        Unknown = 4
    }

    class Debug
    {
        static private ConsoleColor[] stateColors = 
        { 
            ConsoleColor.Gray,
            ConsoleColor.Yellow,
            ConsoleColor.Red,
            ConsoleColor.White,
            ConsoleColor.Cyan
        };

        static private string[] statePrefixes = 
        {
            "Info",
            "Warning",
            "Error",
            "@Dbg",
            ""
        };

        public static void drawDebugLine(debugState state, string value)
        {
            drawDebugLine(state, value, null);
        }

        [StringFormatMethod("format")]
        public static void drawDebugLine(debugState state, string format, params object[] args)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = stateColors[(int)state];

            string value;
            if (args != null)
            {
                value = string.Format(format, args);
            }
            else value = format;
            
            Console.WriteLine("[{0}] [{1}]: {2}", DateTime.Now.ToLocalTime().ToString("H:m:s"),
                statePrefixes[(int)state], value);

            Console.ForegroundColor = currentColor;
        }
    }
}
