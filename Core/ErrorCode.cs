﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronusScript.Core
{
    public enum ErrorCode
    {
        OK =           10,      // No error
        EOF =          11,      // End Of File
        INTR =         12,      // Interrupted
        TOKEN =        13,      // Bad token
        SYNTAX =       14,      // Syntax error
        NOMEM =        15,      // Ran out of memory
        DONE =         16,      // Parsing complete
        ERROR =        17,      // Execution error
        TABSPACE =     18,      // Inconsistent mixing of tabs and spaces
        OVERFLOW =     19,      // Node had too many children
        TOODEEP =      20,      // Too many indentation levels
        DEDENT =       21,      // No matching outer block for dedent
        DECODE =       22,      // Error in decoding into Unicode
        EOFS =         23,      // EOF in triple-quoted string
        EOLS =         24,      // EOL in single-quoted string
        LINECONT =     25,      // Unexpected characters after a line continuation
        BADSINGLE =    27       // Ill-formed single statement input
    }
}
