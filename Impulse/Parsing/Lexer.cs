using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Impulse
{
    class Lexer
    {
        public int IdentifyLine(string line)
        {
            string[] tags;
            if (line.Contains("("))
            {
                if (!line.Contains(")")) return -1;

                tags = line.Split('(');
                if (tags.Length > 2) return -1;

                foreach (var i in tags){
                    Console.WriteLine("i : {0}", i);
                }
            }
            else if (line.Contains("\""))
            {
                tags = line.Split('"');
                if (tags.Length < 1) return -1;
                foreach (var i in tags)
                {
                    Console.WriteLine("i : {0}", i);
                }
            }

            return 0;
        }

        private void CallFunction(string func, object args)
        {
            // Homework :P
        }
    }
}
