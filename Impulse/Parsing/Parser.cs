using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Globalization;

namespace Impulse
{
  class Parser
  {
    private Lexer lex;
    private parseType currentType;
    // private List<Token> operation;

    public Parser()
    {
      lex = new Lexer();
      currentType = parseType.Unknown;
      // operation = new List<Token>();
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
      Variables.printContent();
    }

    private void parseTokens(Token[] tokens)
    {
      VariableObject outVariable = null;

      for(int i = 0; i < tokens.Length && tokens[i].token != null; i++) {
        Debug.drawDebugLine(debugState.Debug, "Token {0}: Current state: {1}, {2} type: {3}", i, this.currentType.ToString(), tokens[i].token, tokens[i].type);

        if(currentType == parseType.Function) {
          if(tokens[i].type == TokenState.Token_Decimal || tokens[i].type == TokenState.Token_Float 
            || tokens[i].type == TokenState.Token_String || tokens[i].type == TokenState.Token_Chars) {

              LocalArguments.addArgument(this.tokenToSystemObject(tokens[i]));
            // operation.Add(tokens[i]);
          }
          else if(tokens[i].type == TokenState.Token_Variable) {
            LocalArguments.addArgument(new VariableObject(tokens[i].token));
          }
          else if(tokens[i].type == TokenState.Token_Brackets_Close) {

            //if(operation[0].type != TokenState.Token_Keyword) {
            //  Debug.drawDebugLine(debugState.Warning, "Not a function! ({0}). :(", operation[0].type.ToString());
            //}
            //else {
            //  Debug.drawDebugLine(debugState.Info, "Method '{0}' call.", operation[0].token);
            //  try {
            //    // Impulse.callStdMethod(arguments[0].token, arguments.Skip(1).ToArray());
            //  }
            //  catch(Exception ex) {
            //    Debug.drawDebugLine(debugState.Error, "Function '{0}' error: {1}", operation[0].token, ex.Message);
            //  }
            //}
            // operation.Clear();

            LocalArguments.clearArguments();
            
            outVariable = null;
            currentType = parseType.Unknown;
          }
        }
        else if(this.currentType == parseType.Variable_operation) {
          Debug.drawDebugLine(debugState.Info, "Current token: {0}.", tokens[i].token);

          if(tokens[i].type == TokenState.Token_Keyword) {
            try {
              Impulse.requestMethod(tokens[i].token);
              this.currentType = parseType.Function;
            }
            catch(Exception e) {
              Debug.drawDebugLine(debugState.Error, "Request function '{0}' error: {1}", tokens[i].token, e.Message);
              this.currentType = parseType.Unknown;
            }
          }
          else if(tokens[i].type == TokenState.Token_Chars || tokens[i].type == TokenState.Token_Decimal || tokens[i].type == TokenState.Token_Float
            || tokens[i].type == TokenState.Token_String) {

            outVariable.setValue(this.tokenToSystemObject(tokens[i]));
          }
          this.currentType = parseType.Unknown;
        }
        else if(this.currentType == parseType.Unknown && tokens[i].type == TokenState.Token_Keyword) {
          // if(tokens[i].token.ToLower() == "define") {
          //   this.currentType = parseType.Definition;
          // }
          // else if(tokens[i].token.ToLower() == "import") {
          //   this.currentType = parseType.Import;
          // }
          if(this.currentType == parseType.Unknown && tokens[i].token.ToLower() == "new") {
            this.currentType = parseType.Variable_definition;
          }
          //else if(Regex.Match(tokens[i].token, "^[_a-z]+[a-zA-Z]+?$").Success) {
          else {
            if(tokens.Length > (i + 1)) {
              // if(tokens[i + 1].type == TokenState.Token_Reference) {
              //  this.currentType = parseType.ClassReference;
              // }
              if(tokens[i + 1].type == TokenState.Token_Brackets) {
                this.currentType = parseType.Function;
              }
              else {
                Debug.drawDebugLine(debugState.Warning, "Unknown keyword declaration token.");
                this.currentType = parseType.Unknown;
                continue;
              }

              try {
                Impulse.requestMethod(tokens[i].token);
              }
              catch(Exception e) {
                Debug.drawDebugLine(debugState.Error, "Request function '{0}' error: {1}", tokens[i].token, e.Message);
                this.currentType = parseType.Unknown;
              }
              // operation.Add(tokens[i]);
            }
          }
          // else Debug.drawDebugLine(debugState.Error, "Can not parse keyword {0}, token type {1}", tokens[i].token, tokens[i].type.ToString());
        }
        else if(this.currentType == parseType.Unknown && tokens[i].type == TokenState.Token_Variable) {
          if(tokens.Length > (i + 1)) {
            // operation.Add(tokens[i]);
            outVariable = new VariableObject(tokens[i].token);

            if(tokens[i + 1].type == TokenState.Token_Operator) {
              // operation.Add(tokens[++i]);
              this.currentType = parseType.Variable_operation;
              i++;
            }
            else {
              Debug.drawDebugLine(debugState.Warning, "What should I do with the variable?");
            }
          }
        }
        else if(this.currentType == parseType.Variable_definition) {
          if(tokens[i].type == TokenState.Token_Variable) {
            Debug.drawDebugLine(debugState.Debug, "Variable definition: {0}", tokens[i].token);

            if(tokens.Length > (i + 1) && tokens[i + 1].type == TokenState.Token_Operator) {
              Debug.drawDebugLine(debugState.Info, "-> Variable assignment");
              outVariable = new VariableObject(tokens[i++].token, true);

              this.currentType = parseType.Variable_operation;
            }
            else {
              Variables.setValue(tokens[i].token, null);
              this.currentType = parseType.Unknown;
            }
          }
          else {
            Debug.drawDebugLine(debugState.Error, "Variable definition is ambiguous!");
          }
        }
      }
    }

    private object tokenToSystemObject(Token token)
    {
      if(token.type == TokenState.Token_Chars) {
        if(token.token.Length > 1) {
          Debug.drawDebugLine(debugState.Info, "Found a character array.");
        }
        else {
          return char.Parse(token.token);
        }
      }
      else if(token.type == TokenState.Token_Decimal) {
        decimal value = 0;

        if(!decimal.TryParse(token.token, out value)) {
          Debug.drawDebugLine(debugState.Error, "Conversion number to decimal failed!");
        }
        else {
          return value;
        }
      }
      else if(token.type == TokenState.Token_Float) {
        float value = 0;

        if(!float.TryParse(token.token, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) {
          Debug.drawDebugLine(debugState.Error, "Convertion number to float failed!");
        }
        else {
          return value;
        }
      }
      else if(token.type == TokenState.Token_String) {
        return token.token;
      }
      return null;
    }
  }
}
