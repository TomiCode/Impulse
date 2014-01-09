using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Impulse
{
    class Lexer
    {
        public LexerReturns IdentifyLine(string line)
        {
            string[] tags;
            if (line.Contains("("))
            {
                if (!line.Contains(")")) return LexerReturns.SYNTAX_ERROR;
                else line = line.Replace(")","");

                tags = line.Split('(');
                if (tags.Length > 2) return LexerReturns.BAD_ARGUMENTS;

                //if(tags[1])

                foreach (var i in tags){
                    Console.WriteLine("i : {0}", i);
                }
            }
            else if (line.Contains("\""))
            {
                tags = line.Split('"');
                if (tags.Length < 3) return LexerReturns.BAD_ARGUMENTS;
                foreach (var i in tags)
                {
                    if (i.Length < 1) continue;
                    string a = i.Trim();
                    Console.WriteLine("i : {0}, Len: {1}", a, a.Length);
                }
            }
            else
            {
                tags = line.Split(' ');

                if (tags.Length == 0) return LexerReturns.SYNTAX_ERROR;
                else if (tags.Length == 1 && tags[0] == "__init") return LexerReturns.Init;

                foreach (var i in tags)
                {
                    if (i.Length < 1) continue;
                    string a = i.Trim();
                    Console.WriteLine("i : {0}, Len: {1}", a, a.Length);
                }
            }

            return LexerReturns.OK;
        }

        private void CallFunction(string func, object args)
        {
            // Homework :P
        }
    }
}
