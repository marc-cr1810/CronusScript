using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CronusScript.Core;
using CronusScript.Objects;

namespace CronusScript.Parser
{
    internal enum DecodingState
    {
        STATE_INIT,
        STATE_RAW,
        STATE_NORMAL
    }

    internal struct TokState
    {
        public long Start;   // Start of current token if not NULL
        public long Cur;     // Next character in buffer

        public ErrorCode Done;  /* E_OK normally, E_EOF at EOF, otherwise error code
                                   NB If done != E_OK, cur must be == inp!!! */
        public FileStream File; // File input
        public bool AtBOL;      // Nonzero if at begin of new line

        public int TabSize;             // Tab Spacing
        public int LineNo;              // Current line number

        public int Indent;              // Current indentation level
        public int[] IndStack;          // Stack of indents
        public int[] AltIndStack;       // Stack of alternate indents
        public char[] ParenStack;       // Stack of parentheses
        public int[] ParenLineNoStack;  // Stack of parentheses line numbers
        public int Level;               /* () [] {} Parentheses nesting level 
                                         Used to allow free continuations inside them */
        public int Pending;             // Pending indents (if > 0) or dedents (if < 0)

        public DecodingState DecodingState; // The decoding state
        public bool DecodingErred; // Whether erred in decoding
        public bool ContLine; // Whether we are in a continuation line

        public bool TypeComments; // Whether to look for type comments

        public TokState(StringObject filepath)
        {
            File = new FileStream(filepath.StringValue, FileMode.Open, FileAccess.Read);
            filepath.IncRef();

            Start = Cur = 0;
            Done = ErrorCode.OK;
            AtBOL = true;
            TabSize = Tokenizer.TabSize;
            LineNo = 0;

            Indent = 0;
            IndStack = new int[Tokenizer.MAX_INDENT];
            AltIndStack = new int[Tokenizer.MAX_INDENT];
            ParenStack = new char[Tokenizer.MAX_PAREN];
            ParenLineNoStack = new int[Tokenizer.MAX_PAREN];
            Level = 0;
            Pending = 0;

            DecodingState = DecodingState.STATE_INIT;
            DecodingErred = false;
            ContLine = false;

            TypeComments = false;
        }
    }

    internal class Tokenizer
    {
        internal const int MAX_INDENT = 100;    // Maximum amount of indentations
        internal const int MAX_PAREN = 200;     // Maximum amount of parentheses
        internal const int TabSize = 8;         // DO NOT CHANGE THIS EVER
        internal const int AltTabSize = 1;      // Alternate tab spacing
        internal const int EOF = -1;

        /* Error helpers */

        /// <summary>
        /// Creates an error token
        /// </summary>
        /// <param name="tok">The parser's tokenizer state</param>
        /// <param name="error">The error code</param>
        /// <returns></returns>
        private static TokenType ErrorToken(ref TokState tok, ErrorCode error)
        {
            tok.Done = error;
            tok.Cur = tok.File.Length;
            tok.File.Position = tok.File.Length;
            return TokenType.ERRORTOKEN;
        }

        /// <summary>
        /// Returns an error token for incorrect indentation
        /// </summary>
        /// <param name="tok">The parser's tokenizer state</param>
        /// <returns></returns>
        private static TokenType IndentError(ref TokState tok)
        {
            return ErrorToken(ref tok, ErrorCode.TABSPACE);
        }

        /// <summary>
        /// Returns an error token for incorrect syntax
        /// </summary>
        /// <param name="tok">The parser's tokenizer state</param>
        /// <param name="msg">The message to be displayed</param>
        /// <returns></returns>
        private static TokenType SyntaxError(ref TokState tok, string msg)
        {
            /// TODO: ERROR HANDLING
            Console.WriteLine("SyntaxError: " + msg);
            tok.Done = ErrorCode.ERROR;
            return TokenType.ERRORTOKEN;
        }

        /* Tokenizer helper functions */

        /// <summary>
        /// Checks to see if a char could be a potential identifier
        /// </summary>
        /// <param name="c">The char to check</param>
        /// <returns></returns>
        private static bool IsPotentialIdentifierStart(int c)
        {
            return ((c >= 'a' && c <= 'z')
               || (c >= 'A' && c <= 'Z')
               || c == '_'
               || (c >= 128));
        }

        /// <summary>
        /// Checks to see if a char could be a potential identifier
        /// </summary>
        /// <param name="c">The char to check</param>
        /// <returns></returns>
        private static bool IsPotentialIdentifierChar(int c)
        {
            return ((c >= 'a' && c <= 'z')
               || (c >= 'A' && c <= 'Z')
               || (c >= '0' && c <= '9')
               || c == '_'
               || (c >= 128));
        }

        /// <summary>
        /// Is a character 0-9 a-f A-F ?
        /// </summary>
        private static bool IsXDigit(char c)
        {
            if ('0' <= c && c <= '9') return true;
            if ('a' <= c && c <= 'f') return true;
            if ('A' <= c && c <= 'F') return true;
            return false;
        }

        private static int DecimalTail(ref TokState tok)
        {
            int c;

            while (true)
            {
                do
                {
                    c = Next(ref tok);
                }
                while (char.IsDigit((char)c));
                if (c != '_')
                {
                    break;
                }
                c = Next(ref tok);
                if (!char.IsDigit((char)c))
                {
                    Back(ref tok, c);
                    SyntaxError(ref tok, "invalid decimal literal");
                    return 0;
                }
            }
            return c;
        }

        /// <summary>
        /// Get the token type from one char
        /// </summary>
        /// <param name="c1">First char</param>
        /// <returns></returns>
        private static TokenType TokenOneChar(int c1)
        {
            switch (c1)
            {
                case '+':
                    return TokenType.ADD;
                case '-':
                    return TokenType.MINUS;
                case '*':
                    return TokenType.STAR;
                case '/':
                    return TokenType.FSLASH;
                case '\\':
                    return TokenType.BSLASH;
                case ',':
                    return TokenType.COMMA;
                case '.':
                    return TokenType.DOT;
                case '=':
                    return TokenType.EQUAL;
                case '>':
                    return TokenType.GREATER;
                case '<':
                    return TokenType.LESS;
                case '@':
                    return TokenType.AT;
                case '%':
                    return TokenType.PERCENT;
                case '&':
                    return TokenType.AMPER;
                case ':':
                    return TokenType.COLON;
                case ';':
                    return TokenType.SEMI;
                case '^':
                    return TokenType.CIRCUMFLEX;
                case '~':
                    return TokenType.TILDE;
                case '|':
                    return TokenType.VBAR;
                case '(':
                    return TokenType.LPAREN;
                case ')':
                    return TokenType.RPAREN;
                case '[':
                    return TokenType.LSQB;
                case ']':
                    return TokenType.RSQB;
                case '{':
                    return TokenType.LBRACE;
                case '}':
                    return TokenType.RBRACE;
            }
            return TokenType.OP;
        }

        /// <summary>
        /// Get the token type from two chars
        /// </summary>
        /// <param name="c1">First char</param>
        /// <param name="c2">Second char</param>
        /// <returns></returns>
        private static TokenType TokenTwoChars(int c1, int c2)
        {
            switch (c1)
            {
                case '+':
                    switch (c2)
                    {
                        case '=':
                            return TokenType.ADDEQUAL;
                        case '+':
                            return TokenType.DOUBLEADD;
                    }
                    break;
                case '-':
                    switch (c2)
                    {
                        case '=':
                            return TokenType.MINUSEQUAL;
                        case '-':
                            return TokenType.DOUBLEMINUS;
                    }
                    break;
                case '*':
                    switch (c2)
                    {
                        case '*':
                            return TokenType.DOUBLESTAR;
                        case '=':
                            return TokenType.STAREQUAL;
                    }
                    break;
                case '/':
                    switch (c2)
                    {
                        case '/':
                            return TokenType.DOUBLEFSLASH;
                        case '=':
                            return TokenType.FSLASHEQUAL;
                    }
                    break;
                case '!':
                    switch (c2)
                    {
                        case '=':
                            return TokenType.NOTEQUAL;
                    }
                    break;
                case '>':
                    switch (c2)
                    {
                        case '=':
                            return TokenType.GREATEREQUAL;
                        case '>':
                            return TokenType.RIGHTSHIFT;
                    }
                    break;
                case '<':
                    switch (c2)
                    {
                        case '=':
                            return TokenType.LESSEQUAL;
                        case '<':
                            return TokenType.LEFTSHIFT;
                        case '>':
                            return TokenType.NOTEQUAL;
                    }
                    break;
                case '.':
                    switch (c2)
                    {
                        case '.':
                            return TokenType.ELLIPSIS;
                    }
                    break;
            }
            return TokenType.OP;
        }

        /// <summary>
        /// Get the token type from three chars
        /// </summary>
        /// <param name="c1">First char</param>
        /// <param name="c2">Second char</param>
        /// <param name="c3">Third char</param>
        /// <returns></returns>
        private static TokenType TokenThreeChars(int c1, int c2, int c3)
        {
            return TokenType.OP;
        }

        /* Tokenizer functions */

        /// <summary>
        /// Get the next char in the file
        /// </summary>
        /// <param name="tok">The parser tokenizer state</param>
        /// <returns></returns>
        private static int Next(ref TokState tok)
        {
            if (tok.File != null)
            {
                int c = tok.File.ReadByte();
                tok.Cur++;

                if (c == EOF)
                {
                    tok.Done = ErrorCode.EOF;
                    return EOF;
                }
                if (c == '\n')
                    tok.LineNo++;
                return c;
            }
            return EOF;
        }

        private static void Back(ref TokState tok, int c)
        {
            if (c != EOF)
            {
                if (--tok.Cur < 0)
                {
                    /// TODO: ERROR HANDLING
                    throw new SystemException("Tokenizer at beginning of file");
                }
                tok.File.Position = tok.Cur;
            }
        }

        private static TokenType Get(ref TokState tok, ref long start, ref long end)
        {
            int c;
            bool blankline = false, nonascii = false;

        nextline:
            tok.Start = 0;
            blankline = false;

            // Get indentation level
            if (tok.AtBOL)
            {
                int col = 0;
                int altcol = 0;
                tok.AtBOL = false;
                for (; ; )
                {
                    c = Next(ref tok);
                    if (c == ' ')
                    {
                        col++;
                        altcol++;
                    }
                    else if (c == '\t')
                    {
                        col = (col / tok.TabSize + 1) * tok.TabSize;
                        altcol = (altcol / AltTabSize + 1) * AltTabSize;
                    }
                    else if (c == 12)
                    { // Control-L (formfeed)
                        col = altcol = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                Back(ref tok, c);
                if (c == '#' || c == '\n' || c == '\\')
                {
                    // Ignore completely
                    blankline = true;
                }
                /* We can't jump back right here since we still
                   may need to skip to the end of a comment */
                if (!blankline && tok.Level == 0)
                {
                    if (col == tok.IndStack[tok.Indent])
                    {
                        // No change
                        if (altcol != tok.AltIndStack[tok.Indent])
                        {
                            return IndentError(ref tok);
                        }
                    }
                    else if (col > tok.IndStack[tok.Indent])
                    {
                        // Indent -- always one
                        if (tok.Indent + 1 >= MAX_INDENT)
                        {
                            return ErrorToken(ref tok, ErrorCode.TOODEEP);
                        }
                        if (altcol <= tok.AltIndStack[tok.Indent])
                        {
                            return IndentError(ref tok);
                        }
                        tok.Pending++;
                        tok.IndStack[++tok.Indent] = col;
                        tok.AltIndStack[tok.Indent] = altcol;
                    }
                    else // col < tok->indstack[tok->indent]
                    {
                        // Dedent -- any number, must be consistent
                        while (tok.Indent > 0 &&
                               col < tok.IndStack[tok.Indent])
                        {
                            tok.Pending--;
                            tok.Indent--;
                        }
                        if (col != tok.IndStack[tok.Indent])
                        {
                            return ErrorToken(ref tok, ErrorCode.DEDENT);
                        }
                        if (altcol != tok.AltIndStack[tok.Indent])
                        {
                            return IndentError(ref tok);
                        }
                    }
                }
            }

            tok.Start = tok.Cur;

            // Return pending indents/dedents
            if (tok.Pending != 0)
            {
                if (tok.Pending < 0)
                {
                    tok.Pending++;
                    return TokenType.DEDENT;
                }
                else
                {
                    tok.Pending--;
                    return TokenType.INDENT;
                }
            }

            // Peek ahead at the next character
            c = Next(ref tok);
            Back(ref tok, c);

        again:
            tok.Start = 0;
            // Skip spaces
            do
            {
                c = Next(ref tok);
            } while (c == ' ' || c == '\t' || c == 12);

            // Set start of the current token
            tok.Start = tok.Cur - 1;

            // Skip comment, unless it's a type comment
            if (c == '#')
            {
                string prefix, p, type_start;

                while (c != EOF && c != '\n')
                {
                    c = Next(ref tok);
                }

                /// TODO: Type comments
                if (tok.TypeComments)
                {

                }
            }

            // Check for EOF and errors now
            if (c == EOF)
            {
                return tok.Done == ErrorCode.EOF ? TokenType.ENDMARKER : TokenType.ERRORTOKEN;
            }

            // Identifier (most frequent token!)
            nonascii = false;
            if (IsPotentialIdentifierStart(c))
            {
                // Process the various legal combinations of b"", r"", u"", and f"".
                bool saw_b = false, saw_r = false, saw_u = false, saw_f = false;
                while (true)
                {
                    if (!(saw_b || saw_u || saw_f) && (c == 'b' || c == 'B'))
                        saw_b = true;
                    else if (!(saw_b || saw_u || saw_r || saw_f)
                             && (c == 'u' || c == 'U'))
                    {
                        saw_u = true;
                    }
                    // ur"" and ru"" are not supported
                    else if (!(saw_r || saw_u) && (c == 'r' || c == 'R'))
                    {
                        saw_r = true;
                    }
                    else if (!(saw_f || saw_b || saw_u) && (c == 'f' || c == 'F'))
                    {
                        saw_f = true;
                    }
                    else
                    {
                        break;
                    }
                    c = Next(ref tok);
                    if (c == '"' || c == '\'')
                    {
                        goto letter_quote;
                    }
                }
                while (IsPotentialIdentifierChar(c))
                {
                    if (c >= 128)
                    {
                        nonascii = true;
                    }
                    c = Next(ref tok);
                }
                Back(ref tok, c);
                if (nonascii)
                {
                    return TokenType.ERRORTOKEN;
                }

                start = tok.Start;
                end = tok.Cur;

                return TokenType.NAME;
            }

            // Newline
            if (c == '\n')
            {
                tok.AtBOL = true;
                if (blankline || tok.Level > 0)
                {
                    goto nextline;
                }
                start = tok.Start;
                end = tok.Cur - 1; // Leave out '\n' out of the string
                tok.ContLine = false;
                return TokenType.NEWLINE;
            }

            // Period or number starting with period?
            if (c == '.')
            {
                c = Next(ref tok);
                if (char.IsDigit((char)c))
                {
                    //goto fraction;
                }
                else if (c == '.')
                {
                    c = Next(ref tok);
                    if (c == '.')
                    {
                        start = tok.Start;
                        end = tok.Cur;
                        return TokenType.ELLIPSIS;
                    }
                    else
                    {
                        Back(ref tok, c);
                    }
                    Back(ref tok, c);
                }
                else
                {
                    Back(ref tok, c);
                }
                start = tok.Start;
                end = tok.Cur;
                return TokenType.DOT;
            }

            // Number
            if (char.IsDigit((char)c))
            {
                if (c == '0')
                {
                    // Hex, octal or binary -- maybe.
                    c = Next(ref tok);
                    if (c == 'x' || c == 'X')
                    {
                        // Hex
                        c = Next(ref tok);
                        do
                        {
                            if (c == '_')
                            {
                                c = Next(ref tok);
                            }
                            if (!IsXDigit((char)c))
                            {
                                Back(ref tok, c);
                                return SyntaxError(ref tok, "invalid hexadecimal literal");
                            }
                            do
                            {
                                c = Next(ref tok);
                            }
                            while (IsXDigit((char)c));
                        }
                        while (c == '_');
                    }
                    else if (c == 'o' || c == 'O')
                    {
                        // Octal
                        c = Next(ref tok);
                        do
                        {
                            if (c == '_')
                            {
                                c = Next(ref tok);
                            }
                            if (c < '0' || c >= '8')
                            {
                                Back(ref tok, c);
                                if (char.IsDigit((char)c))
                                {
                                    return SyntaxError(ref tok, "invalid digit in octal literal");
                                }
                                else
                                {
                                    return SyntaxError(ref tok, "invalid octal literal");
                                }
                            }
                            do
                            {
                                c = Next(ref tok);
                            }
                            while ('0' <= c && c < '8');
                        }
                        while (c == '_');
                        if (char.IsDigit((char)c))
                        {
                            return SyntaxError(ref tok, "invalid digit in octal literal");
                        }
                    }
                    else if (c == 'b' || c == 'B')
                    {
                        // Binary
                        c = Next(ref tok);
                        do
                        {
                            if (c == '_')
                            {
                                c = Next(ref tok);
                            }
                            if (c != '0' && c != '1')
                            {
                                Back(ref tok, c);
                                if (char.IsDigit((char)c))
                                {
                                    return SyntaxError(ref tok, "invalid digit in binary literal");
                                }
                                else
                                {
                                    return SyntaxError(ref tok, "invalid binary literal");
                                }
                            }
                            do
                            {
                                c = Next(ref tok);
                            }
                            while (c == '0' || c == '1');
                        }
                        while (c == '_');
                        if (char.IsDigit((char)c))
                        {
                            return SyntaxError(ref tok, "invalid digit in binary literal");
                        }
                    }
                    else
                    {
                        bool nonzero = false;
                        // maybe old-style octal; c is first char of it
                        // in any case, allow '0' as a literal
                        while (true)
                        {
                            if (c == '_')
                            {
                                c = Next(ref tok);
                                if (!char.IsDigit((char)c))
                                {
                                    Back(ref tok, c);
                                    return SyntaxError(ref tok, "invalid decimal literal");
                                }
                            }
                            if (c != '0')
                            {
                                break;
                            }
                            c = Next(ref tok);
                        }
                        if (char.IsDigit((char)c))
                        {
                            nonzero = true;
                            c = DecimalTail(ref tok);
                            if (c == 0)
                            {
                                return TokenType.ERRORTOKEN;
                            }
                        }
                        if (c == '.')
                        {
                            c = Next(ref tok);
                            //goto fraction;
                        }
                        else if (c == 'e' || c == 'E')
                        {
                            //goto exponent;
                        }
                        else if (c == 'j' || c == 'J')
                        {
                            //goto imaginary;
                        }
                        else if (nonzero)
                        {
                            // Old-style octal: now disallowed.
                            Back(ref tok, c);
                            return SyntaxError(ref tok,
                                               "leading zeros in decimal integer\n" +
                                               "literals are not permitted;\n" +
                                               "use an 0o prefix for octal integers");
                        }
                    }
                }
                else
                {
                    // Decimal
                    c = DecimalTail(ref tok);
                    if (c == 0)
                    {
                        return TokenType.ERRORTOKEN;
                    }
                    else
                    {
                        // Accept floating point numbers
                        if (c == '.')
                        {
                            c = Next(ref tok);
                        fraction:
                            // Fraction
                            if (char.IsDigit((char)c))
                            {
                                c = DecimalTail(ref tok);
                                if (c == 0)
                                {
                                    return TokenType.ERRORTOKEN;
                                }
                            }
                        }
                        if (c == 'e' || c == 'E')
                        {
                            int e;
                        exponent:
                            e = c;
                            // Exponent part
                            c = Next(ref tok);
                            if (c == '+' || c == '-')
                            {
                                c = Next(ref tok);
                                if (!char.IsDigit((char)c))
                                {
                                    Back(ref tok, c);
                                    return SyntaxError(ref tok, "invalid decimal literal");
                                }
                            }
                            else if (!char.IsDigit((char)c))
                            {
                                Back(ref tok, c);
                                Back(ref tok, e);
                                start = tok.Start;
                                end = tok.Cur;
                                return TokenType.NUMBER;
                            }
                            c = DecimalTail(ref tok);
                            if (c == 0)
                            {
                                return TokenType.ERRORTOKEN;
                            }
                        }
                        if (c == 'j' || c == 'J')
                        {
                        // Imaginary part
                        imaginary:
                            c = Next(ref tok);
                        }
                    }
                }
                Back(ref tok, c);
                start = tok.Start;
                end = tok.Cur;
                return TokenType.NUMBER;
            }

            letter_quote:
            // String
            if (c == '\'' || c == '"')
            {
                int quote = c;
                int quote_size = 1; // 1 or 3
                int end_quote_size = 0;

                // Find the quote size and start of the string
                c = Next(ref tok);
                if (c == quote)
                {
                    c = Next(ref tok);
                    if (c == quote)
                    {
                        quote_size = 3;
                    }
                    else
                    {
                        end_quote_size = 1; // empty string was found
                    }
                }
                if (c != quote)
                {
                    Back(ref tok, c);
                }

                // Get rest of string
                while (end_quote_size != quote_size)
                {
                    c = Next(ref tok);
                    if (c == EOF)
                    {
                        if (quote_size == 3)
                        {
                            return ErrorToken(ref tok, ErrorCode.EOFS);
                        }
                        else
                        {
                            return ErrorToken(ref tok, ErrorCode.EOLS);
                        }
                    }
                    if (quote_size == 1 && c == '\n')
                    {
                        return ErrorToken(ref tok, ErrorCode.EOLS);
                    }
                    if (c == quote)
                    {
                        end_quote_size += 1;
                    }
                    else
                    {
                        end_quote_size = 0;
                        if (c == '\\')
                        {
                            Next(ref tok); // Skip escaped char
                        }
                    }
                }

                start = tok.Start;
                end = tok.Cur;
                return TokenType.STRING;
            }

            // Line continuation
            if (c == '\\')
            {
                c = Next(ref tok);
                if (c != '\n')
                {
                    return ErrorToken(ref tok, ErrorCode.LINECONT);
                }
                c = Next(ref tok);
                if (c == EOF)
                {
                    return ErrorToken(ref tok, ErrorCode.EOF);
                }
                else
                {
                    Back(ref tok, c);
                }
                tok.ContLine = true;
                goto again; // Read next line
            }

            // Check for two-character token
            {
                int c2 = Next(ref tok);
                TokenType token = TokenTwoChars(c, c2);
                if (token != TokenType.OP)
                {
                    int c3 = Next(ref tok);
                    TokenType token3 = TokenThreeChars(c, c2, c3);
                    if (token3 != TokenType.OP)
                    {
                        token = token3;
                    }
                    else
                    {
                        Back(ref tok, c3);
                    }
                    start = tok.Start;
                    end = tok.Cur;
                    return token;
                }
                Back(ref tok, c2);
            }

            // Keep track of parentheses nesting level
            switch (c)
            {
                case '(':
                case '[':
                case '{':
                    if (tok.Level >= MAX_PAREN)
                    {
                        return SyntaxError(ref tok, "too many nested parentheses");
                    }
                    tok.ParenStack[tok.Level] = (char)c;
                    tok.ParenLineNoStack[tok.Level] = tok.LineNo;
                    tok.Level++;
                    break;
                case ')':
                case ']':
                case '}':
                    if (tok.Level == 0)
                    {
                        return SyntaxError(ref tok, "unmatched paren");
                    }
                    tok.Level--;
                    int opening = tok.ParenStack[tok.Level];
                    if (!((opening == '(' && c == ')') ||
                          (opening == '[' && c == ']') ||
                          (opening == '{' && c == '}')))
                    {
                        if (tok.ParenLineNoStack[tok.Level] != tok.LineNo)
                        {
                            return SyntaxError(ref tok,
                                               "closing parenthesis does not match\n" +
                                               "opening parenthesis");
                        }
                        else
                        {
                            return SyntaxError(ref tok,
                                               "closing parenthesis does not match\n" +
                                               "opening parenthesis");
                        }
                    }
                    break;
            }

            // Punctuation character
            start = tok.Start;
            end = tok.Cur;
            return TokenOneChar(c);
        }

        public static TokenType GetToken(ref TokState tok, ref long start, ref long end)
        {
            TokenType result = Get(ref tok, ref start, ref end);
            if (tok.DecodingErred)
            {
                result = TokenType.ERRORTOKEN;
                tok.Done = ErrorCode.DECODE;
            }

            return result;
        }

        public static TokenType FillToken(Parser p)
        {
            long start = 0;
            long end = 0;
            TokenType type = GetToken(ref p.Tok, ref start, ref end);

            // Skip '# type: ignore' comments
            while (type == TokenType.TYPE_IGNORE)
                type = GetToken(ref p.Tok, ref start, ref end);



            return TokenType.ENDMARKER;
        }
    }
}
