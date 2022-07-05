using CronusScript.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Objects
{
    internal record StringTokenPair(string Value, TokenType Type);

    internal class TokenObject : RObject
    {
        public readonly string Value;
        public readonly TokenType TokenType;
        public static readonly StringTokenPair[] TokenValues =
        {
            new StringTokenPair("", TokenType.UNKNOWN),
            new StringTokenPair(";", TokenType.ENDMARKER),
            new StringTokenPair("NEWLINE", TokenType.NEWLINE),
            new StringTokenPair("NAME", TokenType.NAME),
            new StringTokenPair("NUMBER", TokenType.NUMBER),
            new StringTokenPair("STRING", TokenType.STRING),
            new StringTokenPair("if", TokenType.IF),
            new StringTokenPair("do", TokenType.DO),
            new StringTokenPair("for", TokenType.FOR),
            new StringTokenPair("else", TokenType.ELSE),
            new StringTokenPair("func", TokenType.FUNC),
            new StringTokenPair("Null", TokenType.NULL),
            new StringTokenPair("True", TokenType.TRUE),
            new StringTokenPair("while", TokenType.WHILE),
            new StringTokenPair("class", TokenType.CLASS),
            new StringTokenPair("async", TokenType.ASYNC),
            new StringTokenPair("await", TokenType.AWAIT),
            new StringTokenPair("false", TokenType.FALSE),
            new StringTokenPair("extension", TokenType.EXTENSION),
            new StringTokenPair("OP", TokenType.OP),
            new StringTokenPair("+", TokenType.ADD),
            new StringTokenPair("-", TokenType.MINUS),
            new StringTokenPair("*", TokenType.STAR),
            new StringTokenPair("/", TokenType.FSLASH),
            new StringTokenPair("\\", TokenType.BSLASH),
            new StringTokenPair(",", TokenType.COMMA),
            new StringTokenPair(".", TokenType.DOT),
            new StringTokenPair("=", TokenType.EQUAL),
            new StringTokenPair(">", TokenType.GREATER),
            new StringTokenPair("<", TokenType.LESS),
            new StringTokenPair("@", TokenType.AT),
            new StringTokenPair("%", TokenType.PERCENT),
            new StringTokenPair("&", TokenType.AMPER),
            new StringTokenPair(":", TokenType.COLON),
            new StringTokenPair(";", TokenType.SEMI),
            new StringTokenPair("^", TokenType.CIRCUMFLEX),
            new StringTokenPair("~", TokenType.TILDE),
            new StringTokenPair("|", TokenType.VBAR),
            new StringTokenPair("(", TokenType.LPAREN),
            new StringTokenPair(")", TokenType.RPAREN),
            new StringTokenPair("[", TokenType.LSQB),
            new StringTokenPair("]", TokenType.RSQB),
            new StringTokenPair("{", TokenType.LBRACE),
            new StringTokenPair("}", TokenType.RBRACE),
            new StringTokenPair("!=", TokenType.NOTEQUAL),
            new StringTokenPair("+=", TokenType.ADDEQUAL),
            new StringTokenPair("-=", TokenType.MINUSEQUAL),
            new StringTokenPair("*=", TokenType.STAREQUAL),
            new StringTokenPair("/=", TokenType.FSLASHEQUAL),
            new StringTokenPair(">=", TokenType.GREATEREQUAL),
            new StringTokenPair("<=", TokenType.LESSEQUAL),
            new StringTokenPair(":=", TokenType.COLONEQUAL),
            new StringTokenPair("++", TokenType.DOUBLEADD),
            new StringTokenPair("--", TokenType.DOUBLEMINUS),
            new StringTokenPair("..", TokenType.ELLIPSIS),
            new StringTokenPair("**", TokenType.DOUBLESTAR),
            new StringTokenPair("//", TokenType.DOUBLEFSLASH),
            new StringTokenPair("<<", TokenType.LEFTSHIFT),
            new StringTokenPair(">>", TokenType.RIGHTSHIFT),
            new StringTokenPair("INDENT", TokenType.INDENT),
            new StringTokenPair("DEDENT", TokenType.DEDENT),
            new StringTokenPair("TYPE_IGNORE", TokenType.TYPE_IGNORE),
            new StringTokenPair("TYPE_COMMENT", TokenType.TYPE_COMMENT),
            new StringTokenPair("ERRORTOKEN", TokenType.ERRORTOKEN),
            new StringTokenPair("N_TOKENS", TokenType.N_TOKENS),
            new StringTokenPair("NT_OFFSET", TokenType.NT_OFFSET)
        };

        public TokenObject(string value, TokenType tokenType) : base("token", $"'{value}'")
        {
            Value = value;
            TokenType = tokenType;
        }

        public static RObject FromQuote(string value)
        {
            foreach (StringTokenPair pair in TokenValues)
            {
                if (pair.Value == value)
                {
                    return new TokenObject(pair.Value, pair.Type);
                }
            }
            return new RObject("null");
        }

        public static RObject FromID(string value)
        {
            object? type;
            if (Enum.TryParse(typeof(TokenType), value, out type))
                return new TokenObject(value, (TokenType)type);
            return new RObject("null");
        }
    }
}
