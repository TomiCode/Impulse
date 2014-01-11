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
        Token_Operator = 5
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
            char[] nextChar = new char[1];
            char[] buffer = new char[128];

            Token[] tokens = new Token[256]; // Only for now.
            int lexPosition = 0;

            while (stream.Read(nextChar, 0, 1) == 1)
            {
                if (state == TokenState.Token_String || state == TokenState.Token_Chars)
                {
                    if (nextChar[0] == '"' && state == TokenState.Token_String 
                        || nextChar[0] == '\'' && state == TokenState.Token_Chars)
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
                        buffer[lexPosition] = nextChar[0];
                        lexPosition++;
                    }
                }
                else if (state == TokenState.Token_Variable || state == TokenState.Token_Keyword)
                {

                }
                else if (nextChar[0] == '"' && state == TokenState.Token_Unknown)
                {
                    state = TokenState.Token_String;
                    continue;
                }
                else if (nextChar[0] == '\'')
                {
                    state = TokenState.Token_Chars;
                    continue;
                }
                else if (nextChar[0] == '$')
                {
                    state = TokenState.Token_Variable;
                    continue;
                }
                else if (nextChar[0] >= 'a' 
                        || nextChar[0] <= 'z'
                        || nextChar[0] >= 'A'
                        || nextChar[0] <= 'Z' )
                {
                    state = TokenState.Token_Keyword;
                    continue;
                }
                else if (nextChar[0] == ' ')
                {

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
