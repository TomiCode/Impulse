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
            Variable_definition = 2,
            ClassReference = 3,
            Method = 4,
            Import = 5,
            Assign = 6,
            Unknown = 7,
            Variable_operation = 8
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
                    else if (tokens[i].token.ToLower() == "new")
                    {
                        this.currentType = parseType.Variable_definition;
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

                        Functions f = new Functions();
                        Object[] o = new Object[1];
                        string function = arguments[0];
                        arguments.Remove(function);

                        o[0] = arguments.ToArray();
                        try
                        {
                            f.GetType().GetMethod(function).Invoke(f, o);
                        }
                        catch (Exception ex)
                        {
                            if ((uint)ex.HResult == (uint)0x80004003)
                            {
                                Debug.drawDebugLine(debugState.Error, "Function {0} is not defined!", function);
                            }
                            else
                            {
                                Debug.drawDebugLine(debugState.Error, "Exception 0x{0:x}", ex.HResult);
                            }
                        }

                        arguments.Clear();
                    }
                    else if (tokens[i].type == TokenState.Token_Brackets 
                        || tokens[i].type == TokenState.Token_Comma)
                    {
                        continue;
                    }
                    else if (tokens[i].type == TokenState.Token_Variable)
                    {
                        Variable.variableType type = new Variable.variableType();
                        string value = getVariableValue(tokens[i].token, out type);

                        if (value != null)
                        {
                            arguments.Add(value);
                        }
                        else
                        {
                            Debug.drawDebugLine(debugState.Error, "{0} argument error: Variable {1} is not defined!", arguments[0], tokens[i].token);
                        }
                    }
                    else
                    {
                        arguments.Add(tokens[i].token);
                    }
                }

                else if (this.currentType == parseType.Variable_definition)
                {
                    if (tokens[i].type == TokenState.Token_Variable)
                    {
                        Debug.drawDebugLine(debugState.Debug, "Variable definition: {0}", tokens[i].token);
                        
                        Variable newVar = new Variable();
                        newVar.type = Variable.variableType.Not_defined;
                        newVar.name = tokens[i].token;
                        newVar.value = "";  // Make sure that's the value of any variable in impulse isn't equal null.
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
                else if(this.currentType == parseType.Variable_operation)
                {
                    string value = "";
                    Variable.variableType type;

                    if ((value = getVariableValue(arguments[0], out type)) == null)
                    {
                        Debug.drawDebugLine(debugState.Warning, "Definition: Variable {0} not defined!", arguments[0]);
                        continue;
                    }
                    else
                    {
                        if (arguments[1] == "=")
                        {
                            if (tokens[i].type == TokenState.Token_Float
                                || tokens[i].type == TokenState.Token_Decimal
                                || tokens[i].type == TokenState.Token_Chars
                                || tokens[i].type == TokenState.Token_String)
                            {
                                Variable.variableType newType;
                                if (type != (newType = getVariableType(tokens[i].type)))
                                {
                                    Debug.drawDebugLine(debugState.Error, "Variable type mess! {0} {1} => {2}",
                                        arguments[0], type.ToString(), newType.ToString());
                                }

                                assignVariableToValue(arguments[0], tokens[i].token, newType);
                            }
                        }
                    }
                    arguments.Clear();
                    this.currentType = parseType.Unknown;
                }
                else if (tokens[i].type == TokenState.Token_Variable)
                {
                    if (tokens.Length > i + 1)
                    {
                        if (tokens[i + 1].type == TokenState.Token_Operator)
                        {
                            Debug.drawDebugLine(debugState.Debug, "Adding variable [{0}] operation: {1} => Arguments len {2}",
                                tokens[i].token, tokens[i + 1].token, arguments.Count);
                            arguments.Add(tokens[i].token);
                            arguments.Add(tokens[i + 1].token);

                            i++;
                            this.currentType = parseType.Variable_operation;
                        }
                    }
                }

                
            }

            //object[] obj = new object[1];
            //obj[0] = args;
            //this.GetType().GetMethod(function).Invoke(this, obj);
        }

        private string getVariableValue(string name, out Variable.variableType type)
        {
            type = Variable.variableType.Not_defined;

            for (int i = 0; i < this.variables.Count; i++)
            {
                if (this.variables[i].name == name)
                {
                    type = this.variables[i].type;
                    return this.variables[i].value;
                }
            }
            return null;
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
