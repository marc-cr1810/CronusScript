using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CronusScript.Core;

namespace CronusScript.Parser
{
    internal class Parser
    {
        /// Parser data

        public bool ErrorIndicator;
        public ErrorCode ErrorCode;
        public bool ParsingStarted;

        public TokState Tok;
        public List<Token> Tokens;

        public int StartingLineNo;
        public int StartingColOffset;

        public int Level;
        public int Fill;
        public int Mark;

        /// Important for the actual parsing

        public const int MAXSTACK = 6000;

        public struct KeywordToken
        {
            public string? Name;
            public TokenType Type;
        }

        // Has to be done because C# requires all array elements to be filled
        // Looks stupid but I can't figure out another method, waste of memory
        private static readonly KeywordToken NullKeyword = new KeywordToken() { Name = null, Type = TokenType.UNKNOWN };
        public static readonly KeywordToken[,] Keywords = new KeywordToken[6, 10]
        {
            { // 0
                NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword
            },
            { // 1
                NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword
            },
            { // 2
                new KeywordToken() { Name = "if", Type = TokenType.IF },
                new KeywordToken() { Name = "do", Type = TokenType.DO },
                NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword
            },
            { // 3
                new KeywordToken() { Name = "for", Type = TokenType.FOR },
                NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword
            },
            { // 4
                new KeywordToken() { Name = "else", Type = TokenType.ELSE },
                new KeywordToken() { Name = "func", Type = TokenType.FUNC },
                new KeywordToken() { Name = "Null", Type = TokenType.NULL },
                new KeywordToken() { Name = "True", Type = TokenType.TRUE },
                NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword
            },
            { // 5
                new KeywordToken() { Name = "while", Type = TokenType.WHILE },
                new KeywordToken() { Name = "class", Type = TokenType.CLASS },
                new KeywordToken() { Name = "async", Type = TokenType.ASYNC },
                new KeywordToken() { Name = "await", Type = TokenType.AWAIT },
                new KeywordToken() { Name = "False", Type = TokenType.FALSE },
                NullKeyword,NullKeyword,NullKeyword,NullKeyword,NullKeyword
            }
        };

        public Parser(TokState tok)
        {
            ErrorIndicator = false;
            ErrorCode = ErrorCode.OK;
            ParsingStarted = false;

            Tok = tok;
            Tokens = new List<Token>();

            StartingLineNo = 0;
            StartingColOffset = 0;

            Level = 0;
            Fill = 0;
            Mark = 0;
        }

        public static void Parse(ref Parser p)
        {
            /* !!! TEMPORARY CODE !!! */
            RuleFile(ref p);
        }

        /*
         * Debug print functions
         */

        [Conditional("DEBUG")]
        private static void PrintTest(ref Parser p, string func, string check, int mark)
        {
            string msg = new string(' ', p.Level);
            msg += $"> {func} [{mark}-{p.Mark}]: {check}";
            Console.WriteLine(msg);
        }

        [Conditional("DEBUG")]
        private static void PrintSuccess(ref Parser p, string func, string check, int mark)
        {
            string msg = $"+ {func} [{mark}-{p.Mark}]: {check} succeeded!".PadRight(p.Level);
            Console.WriteLine(msg);
        }

        [Conditional("DEBUG")]
        private static void PrintFail(ref Parser p, string func, string check, int mark)
        {
            string msg = $"- {func} [{mark}-{p.Mark}]: {check} failed!".PadRight(p.Level);
            Console.WriteLine(msg);
        }

        /*
         * 
         *  All of the code to build an AST
         *
         */


        // file: statements? $
        private static void RuleFile(ref Parser p)
        {
            if (p.Level++ == MAXSTACK)
            {
                p.ErrorIndicator = true;
                /// TODO: ERROR HANDLING
                Console.WriteLine("No memory");
            }
            if (p.ErrorIndicator)
            {
                p.Level--;
                return;
            }

            PrintTest(ref p, "file", "statements?", 0);
        }
    }
}
