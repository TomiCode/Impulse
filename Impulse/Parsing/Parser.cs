using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Impulse
{
    class Parser
    {
        private Lexer lex;
        private parseType currentType;
        private List<Variable> variables;

        public Parser()
        {
            lex = new Lexer();
            currentType = parseType.Unknown;
            variables = new List<Variable>();
        }

        enum parseType
        {
            Definition = 0,
            Function = 1,
            Variable = 2,
            ClassReference = 3,
            Method = 4,
            Import = 5,
            Assign = 6,
            Unknown = 7
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
            List<string> arguments = new List<string>();

            if (validateTokens(tokens) != 0)
            {
                Debug.drawDebugLine(debugState.Error, "Syntax error! Brackets are not closed property!");
                return;
            }

            for (int i = 0; i < tokens.Length && tokens[i].token != null; i++)
            {
                Debug.drawDebugLine(debugState.Debug, "| Token {0}: {1} type: {2} |", i, tokens[i].token, tokens[i].type);
                Debug.drawDebugLine(debugState.Info, "Current parser state: {0}", this.currentType.ToString());

                if(currentType == parseType.Unknown && tokens[i].type == TokenState.Token_Keyword)
                {
                    if (tokens[i].token.ToLower() == "define")
                    {
                        this.currentType = parseType.Definition;
                        
                    }
                    else if (tokens[i].token.ToLower() == "import")
                    {
                        this.currentType = parseType.Import;
                    }
                    else if (tokens[i].token.ToLower() == "stable")
                    {
                        this.currentType = parseType.Variable;
                    }
                    else if (Regex.Match(tokens[i].token, "^[a-z][a-zA-Z0-9]+?$").Success)
                    {
                        if (tokens.Length > i + 1)
                        {
                            if (tokens[i + 1].type == TokenState.Token_Operator)
                            {
                                if (tokens[i + 1].token == "=")
                                {
                                    this.currentType = parseType.Assign;
                                    arguments.Add(tokens[i].token);
                                }
                            }
                        }
                    }
                    else if (Regex.Match(tokens[i].token, "^[_a-z]+[a-zA-Z]+?$").Success)
                    {
                        if (tokens.Length > i + 1)
                        {
                            if (tokens[i + 1].type == TokenState.Token_Reference)
                            {
                                this.currentType = parseType.ClassReference;
                            }
                            else if (tokens[i + 1].type == TokenState.Token_Brackets)
                            {
                                this.currentType = parseType.Function;
                            }
                            arguments.Add(tokens[i].token);
                        }
                    }
                    else
                    {
                        Debug.drawDebugLine(debugState.Error, "Cannot parse keyword {0}, token type {1}",
                                tokens[i].token, tokens[i].type.ToString());
                    }
                    continue;
                }

                else if (this.currentType == parseType.Function)
                {
                    if (tokens[i].type == TokenState.Token_Brackets_Close)
                    {
                        this.currentType = parseType.Unknown;
                        string debug = "";
                        foreach (var a in arguments)
                        {
                            debug += a + " ";
                        }
                        Debug.drawDebugLine(debugState.Info, debug);
                        arguments.Clear();
                    }
                    else if (tokens[i].type == TokenState.Token_Brackets 
                        || tokens[i].type == TokenState.Token_Comma)
                    {
                        continue;
                    }
                    else
                    {
                        arguments.Add(tokens[i].token);
                    }
                }

                
            }

            //object[] obj = new object[1];
            //obj[0] = args;
            //this.GetType().GetMethod(function).Invoke(this, obj);
        }

        public void print(string[] args)
        {
            Console.WriteLine("Function Call: [Print] {0}, {1}", args.Length, args[0]);
        }
    }
}
