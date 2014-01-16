using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Impulse
{
    class Parser
    {
        Lexer lex;
        public Parser()
        {
            lex = new Lexer();
        }


        public void ParseFile(string file)
        {
            Token[] tokens;
            using (StreamReader sReader = new StreamReader(file))
            {
                tokens = lex.LexTextStream(sReader);
            }
            if (tokens == null) return;

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].token != null)
                {
                    Console.WriteLine("| Token {0}: {1} type: {2} |", i, tokens[i].token, tokens[i].type);

                    if (tokens[i].type == TokenState.Token_Keyword)
                    {
                        //Console.WriteLine("-- Parsing keyword [{0}] as {1}", tokens[i].token, tokens[i].type);
                    }
                }
            }
        }
    }
}
