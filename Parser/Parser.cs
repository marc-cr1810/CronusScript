using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public int fill;
        public int mark;

        /// Important for the actual parsing

        private struct KeywordToken
        {
            public string? Name;
            public TokenType Type;
        }

        // Has to be done because C# requires all array elements to be filled
        // Looks stupid but I can't figure out another method, waste of memory
        private static readonly KeywordToken NullKeyword = new KeywordToken() { Name = null, Type = TokenType.UNKNOWN };
        private static readonly KeywordToken[,] Keywords = new KeywordToken[6, 10]
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

            fill = 0;
            mark = 0;
        }

        public void Parse()
        {
            /* !!! TEMPORARY CODE !!! */
            Tokenizer.FillToken(this);
        }
    }
}
