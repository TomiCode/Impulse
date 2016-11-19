using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Impulse
{
  enum TokenState
  {
    Token_Unknown = 0,
    Token_String = 1,
    Token_Chars = 2,
    Token_Keyword = 3,
    Token_Variable = 4,
    Token_Operator = 5,
    Token_Brackets = 6,
    Token_Decimal = 7,
    Token_Float = 8,
    Token_Comma = 9,
    Token_Brackets_Close = 10,
    Token_Reference = 11
  }

  struct Token
  {
    public TokenState type;
    public string token;

    public Token(TokenState type, string token)
    {
      this.type = type;
      this.token = token;
    }
  }

  class Lexer
  {
    TokenState state;
    List<Token> tokens = new List<Token>();

    public Token[] LexTextStream(StreamReader stream)
    {
      char nextChar;
      char[] buffer = new char[128];

      // Token[] tokens = new Token[256]; // Only for now.
      int lexPosition = 0;
      int currentLine = 1;

      while(!stream.EndOfStream) {
        if((nextChar = (char)stream.Read()) == '\r') continue;

        if(state == TokenState.Token_String || state == TokenState.Token_Chars) {
          if(nextChar == '\n') {
            Console.WriteLine("Syntax Error: Line {0} ( {1} )", currentLine, state);
            return null;
          }
          if(nextChar == '"' && state == TokenState.Token_String
              || nextChar == '\'' && state == TokenState.Token_Chars) {

            this.tokens.Add(new Token(state, this.bufferToStringToken(buffer, lexPosition)));

            lexPosition = 0;
            state = TokenState.Token_Unknown; // IDK What the next char is like.
            continue;
          }
          else {
            buffer[lexPosition] = nextChar;
            lexPosition++;
            continue;
          }
        }
        else if(state == TokenState.Token_Variable || state == TokenState.Token_Keyword) {
          if(nextChar == ' '
              || nextChar == '('
              || nextChar == ')'
              || nextChar == '\n'
              || nextChar == '='
              // || nextChar == '+'
              // || nextChar == '<'
              // || nextChar == '>'
              // || nextChar == '*'
              // || nextChar == '/'
              // || nextChar == ':'
              || nextChar == ',') {

            this.tokens.Add(new Token(state, this.bufferToStringToken(buffer, lexPosition)));

            if(nextChar == '=') {
                // || nextChar == '+'
                // || nextChar == '*'
                // || nextChar == '/'
                // || nextChar == '>'
                // || nextChar == '<') {
              state = TokenState.Token_Operator;
              buffer[0] = nextChar;
              lexPosition = 1;
              continue;
            }
            //else if(nextChar == ':' && state == TokenState.Token_Keyword) {
            //  state = TokenState.Token_Reference;

            //  lexPosition = 1;
            //  buffer[0] = nextChar;
            //  continue;
            //}
            else if(nextChar == '(' || nextChar == ')') {
              if(nextChar == '(') state = TokenState.Token_Brackets;
              else state = TokenState.Token_Brackets_Close;

              lexPosition = 1;
              buffer[0] = nextChar;
              continue;
            }
            else if(nextChar == ',') {
              state = TokenState.Token_Comma;
              buffer[0] = nextChar;
              lexPosition = 1;

              continue;
            }
            else {
              state = TokenState.Token_Unknown;
            }

            lexPosition = 0;
            continue;
          }
          else {
            buffer[lexPosition] = nextChar;
            lexPosition++;
            continue;
          }
        }
        else if(state == TokenState.Token_Decimal || state == TokenState.Token_Float) {
          if(nextChar == ' '
              || nextChar == '\n'
              || nextChar == ','
              || nextChar == ')') {

            this.tokens.Add(new Token(state, this.bufferToStringToken(buffer, lexPosition)));

            if(nextChar == ','
                || nextChar == ')') {
              if(nextChar == ',') state = TokenState.Token_Comma;
              else if(nextChar == ')') state = TokenState.Token_Brackets_Close;
              buffer[0] = nextChar;
              lexPosition = 1;

              continue;
            }
            else state = TokenState.Token_Unknown;
            lexPosition = 0;
            continue;
          }
          else if(nextChar == '.') {
            if(state == TokenState.Token_Float) {
              Console.WriteLine("Syntax Error: Line {0} ( {1} )", currentLine, state);
              return null;
            }
            buffer[lexPosition] = nextChar;
            lexPosition++;
            state = TokenState.Token_Float;
            continue;

          }
          else {
            buffer[lexPosition] = nextChar;
            lexPosition++;
            continue;
          }
        }
        else if(state == TokenState.Token_Operator) {
          if(nextChar == '=') {
              // || nextChar == '+'
              // || nextChar == '-') {
            buffer[lexPosition] = nextChar;
            lexPosition++;
          }
          else if(nextChar == '-'
              || (nextChar >= '0' && nextChar <= '9')
              || nextChar == '"'
              || nextChar == '\''
              || nextChar == ' '
              || nextChar >= 'a' && nextChar <= 'z'
              || nextChar >= 'A' && nextChar <= 'Z') {

            this.tokens.Add(new Token(state, this.bufferToStringToken(buffer, lexPosition)));

            if(nextChar == '-'
                || (nextChar >= '0' && nextChar <= '9')) {
              state = TokenState.Token_Decimal;
              buffer[0] = nextChar;
              lexPosition = 1;

              continue;
            }
            else if(nextChar >= 'a' && nextChar <= 'z'
                || nextChar >= 'A' && nextChar <= 'Z') {
              state = TokenState.Token_Keyword; // Maybe? Or not xD
              buffer[0] = nextChar;
              lexPosition = 1;

              continue;
            }
            else if(nextChar == '"') {
              state = TokenState.Token_String;
            }
            else if(nextChar == '\'') {
              state = TokenState.Token_Chars;
            }
            else state = TokenState.Token_Unknown;

            lexPosition = 0;
            continue;
          }
        }
        else if(state == TokenState.Token_Comma) {
          if(nextChar == ' '
              || nextChar >= 'a' && nextChar <= 'z'
              || nextChar >= 'A' && nextChar <= 'Z'
              || nextChar == '$'
              || nextChar >= '0' && nextChar <= '9'
              || nextChar == '-'
              || nextChar == '\''
              || nextChar == '"') {

            this.tokens.Add(new Token(state, this.bufferToStringToken(buffer, lexPosition)));

            if(nextChar == '-'
                || nextChar >= '0' && nextChar <= '9') {
              state = TokenState.Token_Decimal;
              buffer[0] = nextChar;
              lexPosition = 1;

              continue;
            }
            else if(nextChar >= 'a' && nextChar <= 'z'
                || nextChar >= 'A' && nextChar <= 'Z'
                || nextChar == '-') {
              state = TokenState.Token_Keyword;
              buffer[0] = nextChar;
              lexPosition = 1;

              continue;
            }
            else if(nextChar == '"' || nextChar == '\'') {
              if(nextChar == '"') state = TokenState.Token_String;
              else state = TokenState.Token_Chars;

              lexPosition = 0;
              continue;
            }
            else if(nextChar == '$') state = TokenState.Token_Variable;
            else state = TokenState.Token_Unknown;

            lexPosition = 0;
            continue;
          }
        }
        //else if(state == TokenState.Token_Reference) {
        //  if(nextChar != ':' && lexPosition > 1) {

        //    this.tokens.Add(new Token(state, this.bufferToStringToken(buffer, lexPosition)));

        //    if(nextChar >= 'a' && nextChar <= 'z'
        //        || nextChar >= 'A' && nextChar <= 'Z') {
        //      state = TokenState.Token_Keyword;
        //      lexPosition = 1;
        //      buffer[0] = nextChar;
        //      continue;
        //    }
        //    else if(nextChar >= '0' && nextChar <= '9'
        //        || nextChar == '-') {
        //      state = TokenState.Token_Decimal;
        //      lexPosition = 1;
        //      buffer[0] = nextChar;
        //      continue;
        //    }
        //    else if(nextChar == '$') {
        //      state = TokenState.Token_Variable;
        //    }
        //    else if(nextChar == '(') {
        //      state = TokenState.Token_Brackets;

        //      buffer[0] = nextChar;
        //      lexPosition = 1;

        //      continue;
        //    }
        //    else state = TokenState.Token_Unknown;

        //    lexPosition = 0;
        //    continue;

        //  }
        //  else if(nextChar != ':') {
        //    Console.WriteLine("Error Unknown Char ':' on line {0}", currentLine);
        //    //return null;
        //  }
        //  else {
        //    buffer[lexPosition] = nextChar;
        //    lexPosition++;
        //  }

        //}
        else if(state == TokenState.Token_Brackets) {

          this.tokens.Add(new Token(state, this.bufferToStringToken(buffer, lexPosition)));

          Console.WriteLine("Bracket parse: {0}", nextChar);

          if(nextChar >= 'a' && nextChar <= 'z'
              || nextChar >= 'A' && nextChar <= 'Z') {
            state = TokenState.Token_Keyword;

            lexPosition = 1;
            buffer[0] = nextChar;
            continue;
          }
          else if(nextChar >= '0' && nextChar <= '9'
              || nextChar == '-') {
            state = TokenState.Token_Decimal;

            buffer[0] = nextChar;
            lexPosition = 1;
            continue;
          }
          else if(nextChar == '\'' || nextChar == '"') {
            if(nextChar == '"') state = TokenState.Token_String;
            else state = TokenState.Token_Chars;

            lexPosition = 0;
            continue;
          }
          else if(nextChar == '$') {
            state = TokenState.Token_Variable;
            lexPosition = 0;
            continue;
          }
          else if(nextChar == ')') {
            state = TokenState.Token_Brackets_Close;
            buffer[0] = nextChar;
            lexPosition = 1;

            continue;
          }
          else {
            Console.WriteLine("Unknown char {0}", nextChar);
            state = TokenState.Token_Unknown;
            continue;
          }
        }
        else if(state == TokenState.Token_Brackets_Close) {

          this.tokens.Add(new Token(state, this.bufferToStringToken(buffer, lexPosition)));

          // Console.WriteLine("Token " + state.ToString() + " buffer: {0}", tokens[tokenPos].token.Length);

          if(nextChar != ' ') {
            Console.WriteLine("No empty space after {0}", state);
          }

          state = TokenState.Token_Unknown;
          lexPosition = 0;
        }
        else if(nextChar == '\n' && state == TokenState.Token_Unknown) continue;
        else if(nextChar == '"' && state == TokenState.Token_Unknown) {
          state = TokenState.Token_String;
          continue;
        }
        else if(nextChar == '\'') {
          state = TokenState.Token_Chars;
          continue;
        }
        else if(nextChar == '$') {
          state = TokenState.Token_Variable;
          continue;
        }
        else if(nextChar >= 'a' && nextChar <= 'z'
                || nextChar >= 'A' && nextChar <= 'Z'
                || nextChar == '_') {
          buffer[0] = nextChar;
          lexPosition++;
          state = TokenState.Token_Keyword;
          continue;
        }
        else if(nextChar == '-' // This is for a negative number? Hmm...
                || (nextChar >= '0' && nextChar <= '9')) {
          buffer[0] = nextChar;
          lexPosition++;
          state = TokenState.Token_Decimal;
          continue;
        }
        else if(nextChar == '=') {
           // || nextChar == '+'
           // || nextChar == '>'
           // || nextChar == '<'
           // || nextChar == '*'
           // || nextChar == '/') {
          state = TokenState.Token_Operator;
          buffer[0] = nextChar;
          lexPosition++;
          continue;
        }
        else if(nextChar == ',') {
          state = TokenState.Token_Comma;
          buffer[0] = nextChar;
          lexPosition = 1;

          continue;
        }
        else if(nextChar == '('
            || nextChar == ')') {
          if(nextChar == '(') {
            state = TokenState.Token_Brackets;
          }
          else {
            state = TokenState.Token_Brackets_Close;
          }

          buffer[0] = nextChar;
          lexPosition++;
          continue;
        }
        if(nextChar == '\n') currentLine++;
      }
      if(state != TokenState.Token_Unknown) {
        if(state == TokenState.Token_Keyword
            || state == TokenState.Token_Decimal
            || state == TokenState.Token_Float
            || state == TokenState.Token_Operator
            || state == TokenState.Token_Brackets_Close) {
          this.tokens.Add(new Token(state, this.bufferToStringToken(buffer, lexPosition)));
        }
        else {
          Console.WriteLine("Syntax Error: Line {0} ( {1} was not property closed. )", currentLine, state);
        }
      }

      return this.tokens.ToArray();
    }

    private string bufferToStringToken(char[] table, int count)
    {
      string buffer = "";
      for(int i = 0; i < count; i++) {
        buffer += table[i];
      }

      return buffer;
    }
  }
}