using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Impulse
{
    class Parser
    {
        public Lexer lex;

        public Parser()
        {
            lex = new Lexer();
        }

        public void ParseFile(string file)
        {
            string currentLine;
            using (StreamReader sReader = new StreamReader(file))
            {
                while ((currentLine = sReader.ReadLine()) != null)
                {
                    if (currentLine.Length == 0) continue;
                    currentLine = currentLine.Trim(); // Remove all white characters.

                    Console.WriteLine("-> {0}", currentLine);
                    Console.WriteLine(lex.IdentifyLine(currentLine));
                }
            }
        }

        private void ParseLine(string line)
        {
            
        }
    }
}
