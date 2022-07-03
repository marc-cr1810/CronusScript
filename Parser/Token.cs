using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronusScript.Parser
{
    internal enum TokenType
    {
        UNKNOWN = -1,   // Unknown token type; for errors
        ENDMARKER,      // Endmarker token
        NEWLINE,        // Newline token
        NAME,           // Variable names
        NUMBER,         // Number token type
        STRING,         // String token type

		/* Keyword tokens */
		IF,             // if
		DO,             // do
		FOR,            // for
		ELSE,           // else
		FUNC,           // func
		NULL,           // Null
		TRUE,           // True
		WHILE,          // while
		CLASS,          // class
		ASYNC,          // async
		AWAIT,          // await
		FALSE,          // False
		EXTENSION,      // extension

		/* Operator tokens */
		OP,             // Blank operator value, often means unknown operator
		ADD,            // +
		MINUS,          // -
		STAR,           // *
		FSLASH,         // /
		BSLASH,         // "\"
		COMMA,          // ,
		DOT,            // .
		EQUAL,          // =
		GREATER,        // >
		LESS,           // <
		AT,             // @
		PERCENT,        // %
		AMPER,          // &
		COLON,          // :
		SEMI,           // ;
		CIRCUMFLEX,     // ^
		TILDE,          // ~
		VBAR,           // |
		LPAREN,         // (
		RPAREN,         // )
		LSQB,           // [
		RSQB,           // ]
		LBRACE,         // {
		RBRACE,         // }

		/* Double char operator tokens */
		NOTEQUAL,       // "!=", <>
		ADDEQUAL,       // +=
		MINUSEQUAL,     // -=
		STAREQUAL,      // *=
		FSLASHEQUAL,    // /=
		GREATEREQUAL,   // >=
		LESSEQUAL,      // <=
		DOUBLEADD,      // ++
		DOUBLEMINUS,    // --
		ELLIPSIS,       // ..
		DOUBLESTAR,     // **
		DOUBLEFSLASH,   // //
		LEFTSHIFT,      // <<
		RIGHTSHIFT,     // >>

		/* Miscellaneous tokens */
		INDENT,
		DEDENT,
		TYPE_IGNORE,
		TYPE_COMMENT,
		ERRORTOKEN,
		N_TOKENS,
		NT_OFFSET = 256
	}

    internal struct Token
    {
		TokenType type;		// What token is it
		Object value;       // StringObject value of the token
		int LineNo, ColOffset, EndLineNo, EndColOffset; // Line and column position of the token
    }
}
