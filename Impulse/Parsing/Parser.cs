using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Impulse
{
  struct Argument
  {
    public string value;
    public TokenState type;

    public Argument(TokenState type, string value)
    {
      this.value = value;
      this.type = type;
    }

    public Argument(string value)
    {
      this.value = value;
      this.type = TokenState.Token_String;
    }

    public Argument(Token token)
    {
      this.value = token.token;
      this.type = token.type;
    }
  }

  class Parser
  {
    private Lexer lex;
    private parseType currentType;
    private List<Argument> arguments;
    private Variables variables;

    public Parser()
    {
      lex = new Lexer();
      currentType = parseType.Unknown;
      arguments = new List<Argument>();
      variables = new Variables();
    }

    public enum parseType
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
      for(int i = 0; i < tokens.Length; i++) {
        if(tokens[i].token != null) {
          if(tokens[i].type == TokenState.Token_Brackets) brackets++;
          else if(tokens[i].type == TokenState.Token_Brackets_Close) brackets--;
        }
      }
      return brackets;
    }

    public void ParseFile(string file)
    {
      Token[] tokens;
      using(StreamReader sReader = new StreamReader("scripts/test.imp")) {
        tokens = lex.LexTextStream(sReader);
      }

      if(tokens == null) return;
      if(validateTokens(tokens) != 0) {
        Debug.drawDebugLine(debugState.Error, "Syntax error! Brackets are not closed property!");
        return;
      }

      this.parseTokens(tokens);
      this.variables.printAllVariables();
    }

    private void parseTokens(Token[] tokens)
    {
      for(int i = 0; i < tokens.Length && tokens[i].token != null; i++) {
        Debug.drawDebugLine(debugState.Debug, "Token {0}: Current state: {1}, {2} type: {3}", i, this.currentType.ToString(), tokens[i].token, tokens[i].type);

        if(currentType == parseType.Function) {
          if(tokens[i].type == TokenState.Token_Decimal || tokens[i].type == TokenState.Token_Float || tokens[i].type == TokenState.Token_String) {
            arguments.Add(new Argument(tokens[i]));
          }
          else if(tokens[i].type == TokenState.Token_Brackets_Close) {
            if(arguments[0].type != TokenState.Token_Keyword) {
              Debug.drawDebugLine(debugState.Warning, "Not a function! ({0}).", arguments[0].type.ToString());
            }
            else {
              Debug.drawDebugLine(debugState.Info, "Method '{0}' call.", arguments[0].value);
              try {
                Impulse.callStdMethod(arguments[0].value, arguments.Skip(1).ToArray());
              }
              catch(Exception ex) {
                Debug.drawDebugLine(debugState.Error, "Function '{0}' error: {1}", arguments[0].value, ex.Message);
              }
            }
            arguments.Clear();
            currentType = parseType.Unknown;
          }
        }
        else if(currentType == parseType.Unknown && tokens[i].type == TokenState.Token_Keyword) {
          if(tokens[i].token.ToLower() == "define") {
            this.currentType = parseType.Definition;
          }
          else if(tokens[i].token.ToLower() == "import") {
            this.currentType = parseType.Import;
          }
          else if(tokens[i].token.ToLower() == "new") {
            this.currentType = parseType.Variable_definition;
          }
          else if(Regex.Match(tokens[i].token, "^[_a-z]+[a-zA-Z]+?$").Success) {
            if(tokens.Length > i + 1) {
              if(tokens[i + 1].type == TokenState.Token_Reference) {
                this.currentType = parseType.ClassReference;
              }
              else if(tokens[i + 1].type == TokenState.Token_Brackets) {
                this.currentType = parseType.Function;
              }
              arguments.Add(new Argument(tokens[i].type, tokens[i].token));
            }
          }
          else Debug.drawDebugLine(debugState.Error, "Can not parse keyword {0}, token type {1}", tokens[i].token, tokens[i].type.ToString());
          continue;
        }
        else if(this.currentType == parseType.Unknown && tokens[i].type == TokenState.Token_Variable) {
          if(tokens.Length > i + 1) {
            arguments.Add(new Argument(tokens[i].type, tokens[i].token));

            if(tokens[i + 1].type == TokenState.Token_Operator) {
              i++;
              arguments.Add(new Argument(tokens[i].type, tokens[i].token));
              this.currentType = parseType.Variable_operation;
            }
            else {
              Debug.drawDebugLine(debugState.Warning, "Weird, something is wrong with the variable.");
            }
          }
        }
        else if(this.currentType == parseType.Variable_definition) {
          if(tokens[i].type == TokenState.Token_Variable) {
            Debug.drawDebugLine(debugState.Debug, "Variable definition: {0}", tokens[i].token);

            if(tokens.Length > i + 1 && tokens[i + 1].type == TokenState.Token_Operator) {
              Debug.drawDebugLine(debugState.Info, "Variable waiting for assing..");
              arguments.Add(new Argument(tokens[i].type, tokens[i].token));
            }
            else {
              variables.addVariable(tokens[i].token);
              //arguments.Clear();

              this.currentType = parseType.Unknown;
            }
          }
          else if(tokens[i].type == TokenState.Token_Operator && tokens[i].token == "=") {
            i++;
            Debug.drawDebugLine(debugState.Info, "Variable [{0}] was defined with [{1}] type {2}", arguments[0].value, tokens[i].token, tokens[i].type.ToString());
            variables.addVariable(arguments[0].value, tokens[i].token, variables.variableType(tokens[i].type));

            arguments.Clear();
            this.currentType = parseType.Unknown;
          }
          else {
            Debug.drawDebugLine(debugState.Error, "Variable definition is ambiguous!");
          }
        }
        else if(this.currentType == parseType.Variable_operation) {
          Debug.drawDebugLine(debugState.Info, "Operation {1} {0}", tokens[i].token, arguments[1].value);
        }
      }
    }
  }
}
