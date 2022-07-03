using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CronusScript.Objects;
using CronusScript.Core;

namespace CronusScript.Parser
{
    internal class Generator
    {
        private static TokenType TokenizerError(ref Parser p)
        {
            /// TODO: ERROR HANDLING 
            ///     CHECK IF AN ERROR OCCURRED; IF SO RETURN UNKNOWN
            ///     
            string msg = "";
            string errtype = "SyntaxError";
            switch (p.Tok.Done)
            {
                case ErrorCode.TOKEN:
                    msg = "invalid token";
                    break;
                case ErrorCode.EOFS:
                    msg = "EOF while scanning triple-quoted string literal";
                    break;
                case ErrorCode.EOLS:
                    msg = "EOL while scanning string literal";
                    break;
                case ErrorCode.EOF:
                    msg = "unexpected EOF while parsing";
                    break;
                case ErrorCode.DEDENT:
                    errtype = "IndentationError";
                    msg = "unindent does not match any outer indentation level";
                    break;
                case ErrorCode.NOMEM:
                    msg = "Ran out of memory";
                    break;
                case ErrorCode.TABSPACE:
                    errtype = "TabError";
                    msg = "inconsistent use of tabs and spaces in indentation";
                    break;
                case ErrorCode.TOODEEP:
                    errtype = "IndentationError";
                    msg = "too many levels of indentation";
                    break;
                case ErrorCode.LINECONT:
                    msg = "unexpected character after line continuation character";
                    break;
                default:
                    msg = "unknown parsing error";
                    break;
            }

            /// TODO: ERROR HANDLING
            Console.WriteLine($"{errtype}: {msg}");

            return TokenType.UNKNOWN;
        }

        private static TokenType GetKeywordOrNameType(ref Parser p, string name)
        {
            if (name.Length >= Parser.Keywords.GetLength(0) || Parser.Keywords[name.Length,0].Type == TokenType.UNKNOWN)
            {
                return TokenType.NAME;
            }
            for (int i = 0; i < Parser.Keywords.GetLength(1); i++)
            {
                Parser.KeywordToken k = Parser.Keywords[name.Length, i];
                if (k.Type == TokenType.UNKNOWN)
                    break;
                if (k.Name == name)
                {
                    return k.Type;
                }
            }
            return TokenType.NAME;
        }


        public static TokenType FillToken(Parser p)
        {
            long start = 0;
            long end = 0;
            TokenType type = Tokenizer.GetToken(ref p.Tok, ref start, ref end);

            // Skip '# type: ignore' comments
            while (type == TokenType.TYPE_IGNORE)
                type = Tokenizer.GetToken(ref p.Tok, ref start, ref end);

            p.ParsingStarted = true;

            Token t = new Token();
            t.Type = type == TokenType.NAME ? GetKeywordOrNameType(ref p, p.Tok.GetValue()) : type;
            t.Value = new StringObject(p.Tok.GetValue());
            if (t.Value == null)
                return TokenType.UNKNOWN;

            int lineno = type == TokenType.STRING ? p.Tok.FirstLineNo : p.Tok.LineNo;
            long line_start = type == TokenType.STRING ? p.Tok.MultiLineStart : p.Tok.LineStart;
            int end_lineno = p.Tok.LineNo;
            int col_offset = -1;
            int end_col_offset = -1;
            if (start != 0 && start >= line_start)
            {
                col_offset = (int)(start - line_start);
            }
            if (end != 0 && end >= p.Tok.LineStart)
            {
                end_col_offset = (int)(end - p.Tok.LineStart);
            }

            t.LineNo = p.StartingLineNo + lineno;
            t.ColOffset = p.Tok.LineNo == 1 ? p.StartingColOffset + col_offset : col_offset;
            t.EndLineNo = p.StartingLineNo + end_lineno;
            t.EndColOffset = p.Tok.LineNo == 1 ? p.StartingColOffset + end_col_offset : end_col_offset;

            p.Tokens.Add(t);
            p.Fill += 1;

            if (type == TokenType.ERRORTOKEN)
            {
                if (p.Tok.Done == Core.ErrorCode.DECODE)
                {
                    /// TODO: ERROR HANDLING
                    Console.WriteLine("SystemError: unknown error");
                    return TokenType.UNKNOWN;
                }
                return TokenizerError(ref p);
            }

            return TokenType.ENDMARKER;
        }

        public static void RunParser(StringObject filepath)
        {
            TokState tok = new TokState(filepath);

            Parser p = new Parser(tok);

            p.Parse();
        }
    }
}
