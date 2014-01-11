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
        Token_Decimal = 7
    }

    struct Token
    {
        public TokenState type;
        public string token;
    }

    class Lexer
    {
        TokenState state;

        public Token[] LexTextStream(StreamReader stream)
        {
            char nextChar;
            char[] buffer = new char[128];

            Token[] tokens = new Token[256]; // Only for now.
            int lexPosition = 0;

            while ((nextChar = (char)stream.Read()) != 0)
            {
                if (state == TokenState.Token_String || state == TokenState.Token_Chars)
                {
                    if (nextChar == '\n') return null; // Exception! Someone does not closed the string!
                    if (nextChar == '"' && state == TokenState.Token_String 
                        || nextChar == '\'' && state == TokenState.Token_Chars)
                    {
                        int tokenPos = getTokenTableIndex(tokens);
                        if (tokenPos == -1) return null; // Exception! But not now xD

                        tokens[tokenPos] = new Token();
                        tokens[tokenPos].token = bufferToStringToken(buffer, lexPosition);
                        tokens[tokenPos].type = state; // The lexer know that xD
                        
                        lexPosition = 0;
                        state = TokenState.Token_Unknown; // IDK What the next char is like.
                        continue;
                    }
                    else
                    {
                        buffer[lexPosition] = nextChar;
                        lexPosition++;
                    }
                }
                else if (state == TokenState.Token_Variable || state == TokenState.Token_Keyword)
                {
                    if(nextChar == ' '
                        || nextChar == '('
                        || nextChar == '\n'
                        || nextChar == '='
                        || nextChar == '+'
                        || nextChar == '<'
                        || nextChar == '>' 
                        || nextChar == '*'
                        || nextChar == '/')
                    {
                        int tokenPos = getTokenTableIndex(tokens);
                        if (tokenPos == -1) return null; // Buffer overflow xD

                        tokens[tokenPos] = new Token();
                        tokens[tokenPos].token = bufferToStringToken(buffer, lexPosition);
                        tokens[tokenPos].type = state;

                        if(nextChar == '='
                            || nextChar == '+'
                            || nextChar == '*'
                            || nextChar == '/'
                            || nextChar == '>'
                            || nextChar == '<')
                        {
                            state = TokenState.Token_Operator;
                            buffer[0] = nextChar;
                            lexPosition = 1;
                            continue;
                        }
                        else if(nextChar == '(')
                        {
                            state = TokenState.Token_Brackets;
                        }
                        else
                        {
                            state = TokenState.Token_Unknown;
                        }
                        lexPosition = 0;
                        continue;
                    }
                }
                else if (nextChar == '"' && state == TokenState.Token_Unknown)
                {
                    state = TokenState.Token_String;
                    continue;
                }
                else if (nextChar == '\'')
                {
                    state = TokenState.Token_Chars;
                    continue;
                }
                else if (nextChar == '!')
                {
                    state = TokenState.Token_Variable;
                    continue;
                }
                else if (nextChar >= 'a' 
                        || nextChar <= 'z'
                        || nextChar >= 'A'
                        || nextChar <= 'Z' )
                {
                    state = TokenState.Token_Keyword;
                    continue;
                }
            }

            return tokens;
        }

        private int getTokenTableIndex(Token[] table)
        {
            for (int i = 0; i < table.Length; i++)
            {
                if(table[i].token == "" || table[i].token == null) return i;
            }
            return -1;
        }

        private string bufferToStringToken(char[] table, int count)
        {
            string buffer = "";
            for (int i = 0; i < count; i++)
            {
                buffer += table[i];
            }

            return buffer;
        }
    }
}
