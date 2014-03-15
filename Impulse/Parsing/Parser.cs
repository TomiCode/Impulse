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

        enum Parser_State
        {
            Type_Unknown = 0,
            Type_Function = 1,
            Type_Definition = 2
        }

        private int validateTokens(Token[] tokens)
        {
            int brackets = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].token != null)
                {
                    if (tokens[i].type == TokenState.Token_Brackets)
                    {
                        brackets++;
                    }
                    else if (tokens[i].type == TokenState.Token_Brackets_Close)
                    {
                        brackets--;
                    }
                }
            }
            return brackets;
        }

        public void ParseFile(string file)
        {
            Token[] tokens;
            using (StreamReader sReader = new StreamReader("scripts/test.imp"))
            {
                tokens = lex.LexTextStream(sReader);
            }
            if (tokens == null) return;
            string[] args = new string[128];
            int argPosition = 0;

            if (validateTokens(tokens) != 0)
            {
                Debug.drawDebugLine(debugState.Error, "Syntax error! Brackets are not closed property!");
                return;
            }

            Parser_State state = new Parser_State();
            string function = "";

            state = Parser_State.Type_Unknown;

            for (int i = 0; i < tokens.Length && tokens[i].token != null; i++)
            {
                //Console.WriteLine("| Token {0}: {1} type: {2} |", i, tokens[i].token, tokens[i].type);
                Debug.drawDebugLine(debugState.Debug, string.Format("| Token {0}: {1} type: {2} |", i, tokens[i].token, tokens[i].type));

                if(tokens[i].type == TokenState.Token_Brackets_Close) 
                {
                    state = Parser_State.Type_Unknown;
                    continue;

                }
                if (state == Parser_State.Type_Unknown)
                {
                    if (tokens[i].type == TokenState.Token_Keyword)
                    {
                        state = Parser_State.Type_Function;
                        function = tokens[i].token;
                    }
                    else if (tokens[i].type == TokenState.Token_Keyword && tokens[i].token.ToLower() == "define")
                    {
                        Debug.drawDebugLine(debugState.Debug, "Parsing define keyword");
                        state = Parser_State.Type_Definition;

                    }
                }
                else if (state == Parser_State.Type_Definition)
                {
                    Debug.drawDebugLine(debugState.Debug, 
                        string.Format("Use Definiton: {0} {1}", tokens[i].token, tokens[i].type.ToString()));

                    //if (tokens[i].type == TokenState.Token_Keyword) state = Parser_State.Type_Function;
                }
                else if (state == Parser_State.Type_Function)
                {
                    if (tokens[i].type == TokenState.Token_Chars || tokens[i].type == TokenState.Token_String || tokens[i].type == TokenState.Token_Decimal)
                    {
                        args[argPosition] = tokens[i].token;

                        Debug.drawDebugLine(debugState.Debug, 
                        string.Format("Function {0} Got new argument {1}, len: {2}", function, args[argPosition], args[argPosition].Length));

                        argPosition++;
                    }
                    else
                    {
                        //if (tokens[i].type == TokenState.Token_Keyword && tokens[i].token == "define")
                        //{
                        //    state = Parser_State.Type_Definition;
                        //}
                        //Console.WriteLine("Nothing!@!!");
                    }
                }
            }
            object[] obj = new object[1];
            obj[0] = args;

            //this.GetType().GetMethod(function).Invoke(this, obj);
        }


        public void print(string[] args)
        {
            Console.WriteLine("Function Call: [Print] {0}, {1}", args.Length, args[0]);
        }
    }
}
