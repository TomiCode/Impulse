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

                else if (this.currentType == parseType.Variable)
                {
                    if (tokens[i].type == TokenState.Token_Variable)
                    {
                        Debug.drawDebugLine(debugState.Debug, "Variable definition: {0}", tokens[i].token);
                        
                        Variable newVar = new Variable();
                        newVar.type = Variable.variableType.Not_defined;
                        newVar.name = tokens[i].token;
                        variables.Add(newVar);

                        if (tokens.Length > i + 1 &&
                            tokens[i + 1].type == TokenState.Token_Operator)
                        { 
                            Debug.drawDebugLine(debugState.Info, "Variable definition with assign");
                            arguments.Add(tokens[i].token);
                        }
                        else this.currentType = parseType.Unknown;
                    }
                    else if (tokens[i].type == TokenState.Token_Operator && tokens[i].token == "=")
                    {
                        i += 1;
                        Debug.drawDebugLine(debugState.Info, "Variable [{0}] was defined with [{1}] type {2}",
                            arguments[0], tokens[i].token, tokens[i].type.ToString());

                        Variable.variableType currentTokenType = getVariableType(tokens[i].type);
                        if (currentTokenType == Variable.variableType.Not_defined)
                        {
                            Debug.drawDebugLine(debugState.Warning, "Variable {0} got type {1}", arguments[0], currentTokenType.ToString());
                        }

                        assignVariableToValue(arguments[0], tokens[i].token, currentTokenType);
                        arguments.Clear();
                        this.currentType = parseType.Unknown;
                    }
                    else
                    {
                        Debug.drawDebugLine(debugState.Error, "Variable definition is ambiguous!");
                    }
                }

                
            }

            //object[] obj = new object[1];
            //obj[0] = args;
            //this.GetType().GetMethod(function).Invoke(this, obj);
        }

        private bool assignVariableToValue(string name, string value, Variable.variableType type)
        {
            for (int i = 0; i < this.variables.Count; i++)
            {
                if (this.variables[i].name == name)
                {
                    this.variables[i].value = value;
                    this.variables[i].type = type;
                    return true;
                }
            }
            return false;
        }

        private Variable.variableType getVariableType(TokenState state)
        {
            if (state == TokenState.Token_String) return Variable.variableType.String;
            else if (state == TokenState.Token_Chars) return Variable.variableType.Chars;
            else if (state == TokenState.Token_Decimal) return Variable.variableType.Decimal;
            else if (state == TokenState.Token_Float) return Variable.variableType.Float;
            else return Variable.variableType.Not_defined;
        }

        public void print(string[] args)
        {
            Console.WriteLine("Function Call: [Print] {0}, {1}", args.Length, args[0]);
        }

        //
        // DEBUG Only.
        //

        public void showAllVariables()
        {
            Console.WriteLine("\n\n== System Variables\n");
            foreach (var thisVar in variables)
            {
                Console.WriteLine(thisVar.ToString());
            }
            Console.WriteLine("\n== End of System Variables\n");
        }
    }
}
