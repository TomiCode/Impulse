using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Impulse
{
    enum LexerReturns
    {
        OK = 0,
        BAD_ARGUMENTS = -1,
        FUNCTION_NOT_FOUND = -2,
        SYNTAX_ERROR = -3,
        Init = 1
    }
}
