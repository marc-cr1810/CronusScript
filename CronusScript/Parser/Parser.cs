using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using CronusScript.Core;

namespace CronusScript.Parser
{
    internal struct GrowableCommentList
    {
        public struct Item
        {
            public int LineNo;
            public string Comment; // The " <tag>" in "# type: ignore <tag>"
        }
        public List<Item> Items;

        public int Count()
        {
            return Items.Count;
        }
    }

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

        public GrowableCommentList TypeIgnoreComments;

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

            TypeIgnoreComments.Items = new List<GrowableCommentList.Item>();

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
            string msg = new string(' ', p.Level);
            msg += $"+ {func} [{mark}-{p.Mark}]: {check} succeeded!";
            Console.WriteLine(msg);
        }

        [Conditional("DEBUG")]
        private static void PrintFail(ref Parser p, string func, string check, int mark)
        {
            string msg = new string(' ', p.Level);
            msg += $"- {func} [{mark}-{p.Mark}]: {check} failed!";
            Console.WriteLine(msg);
        }

        /*
         * 
         *  All of the code to build an AST
         *
         */


        // file: statements? $
        private static ModType? RuleFile(ref Parser p)
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
                return null;
            }
            ModType? result = null;
            int mark = p.Mark;

            { // statements? $
                if (p.ErrorIndicator)
                {
                    p.Level--;
                    return null;
                }
                PrintTest(ref p, "file", "statements? $", mark);
                StmtSeq? a;
                Token? endmarker_var;
                if (
                    ((a = RuleStatements(ref p)) != null && !p.ErrorIndicator) // statements?
                    &&
                    ((endmarker_var = Generator.ExpectToken(ref p, TokenType.ENDMARKER)) != null) // token='ENDMARKER'
                )
                {
                    PrintSuccess(ref p, "file", "statements? $", mark);
                    result = Generator.MakeModule(ref p, a);
                    if (result == null) /// TODO: ERROR HANDLING Check for errors
                    {
                        p.ErrorIndicator = true;
                        p.Level--;
                        return null;
                    }
                    goto done;
                }
                p.Mark = mark;
                PrintFail(ref p, "file", "statements? $", mark);
            }
            result = null;
        done:
            p.Level--;
            return result;
        }

        // statements: statement+
        private static StmtSeq? RuleStatements(ref Parser p)
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
                return null;
            }
            StmtSeq? result = null;
            int mark = p.Mark;

            { // statement+
                if (p.ErrorIndicator)
                {
                    p.Level--;
                    return null;
                }
                PrintTest(ref p, "statements", "statement+", mark);
                StmtSeqSeq? a;
                if (
                    ((a = LoopRule3(ref p)) != null)
                )
                {
                    PrintSuccess(ref p, "statements", "statement+", mark);
                    result = a.Value.Flatten();
                    if (result == null)  /// TODO: ERROR HANDLING Check for errors
                    {
                        p.ErrorIndicator = true;
                        p.Level--;
                        return null;
                    }
                    goto done;
                }

                p.Mark = mark;
                PrintFail(ref p, "statements", "statement+", mark);
            }

            result = null;
        done:
            p.Level--;
            return result;
        }

        // looprule_3: statement
        private static StmtSeqSeq? LoopRule3(ref Parser p)
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
                return null;
            }

            StmtSeq? result = null;
            int mark = p.Mark;
            List<StmtSeq?> children = new List<StmtSeq?>();
            int n = 0;

            { // statement
                if (p.ErrorIndicator)
                {
                    p.Level--;
                    return null;
                }
                PrintTest(ref p, "looprule_3", "statement", mark);
                StmtSeq? statement_var;
                while (
                    ((statement_var = RuleStatement(ref p)) != null) // statement
                )
                {
                    result = statement_var;
                    children.Add(result);
                    n++;
                    mark = p.Mark;
                }
                p.Mark = mark;
                PrintFail(ref p, "looprule_3", "statement", mark);
            }
            if (n == 0 || p.ErrorIndicator)
            {
                p.Level--;
                return null;
            }

            StmtSeqSeq seq = new StmtSeqSeq();
            seq.Elements = children.ToArray();
            p.Level--;
            return seq;
        }

        // statement: compound_stmt | simple_stmts
        private static StmtSeq? RuleStatement(ref Parser p)
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
                return null;
            }
            StmtSeq? result = null;
            int mark = p.Mark;

            { // compound_stmt
                if (p.ErrorIndicator)
                {
                    p.Level--;
                    return null;
                }
                PrintTest(ref p, "statement", "compound_stmt", mark);
                StmtType? a;
                if (
                    ((a = RuleCompoundStmt(ref p)) != null) // compound_stmt
                )
                {
                    PrintSuccess(ref p, "statement", "compound_stmt", mark);
                    result = a.Value.SingletonSeq();
                    if (result == null)  /// TODO: ERROR HANDLING Check for errors
                    {
                        p.ErrorIndicator = true;
                        p.Level--;
                        return null;
                    }
                    goto done;
                }
                p.Mark = mark;
                PrintFail(ref p, "statement", "compound_stmt", mark);
            }
            { // simple_stmts
                if (p.ErrorIndicator)
                {
                    p.Level--;
                    return null;
                }
                PrintTest(ref p, "statement", "simple_stmts", mark);
                StmtSeq? a;
                if (
                    ((a = RuleSimpleStmts(ref p)) != null)
                )
                {
                    PrintSuccess(ref p, "statement", "simple_stmts", mark);
                    result = a;
                    if (result == null)  /// TODO: ERROR HANDLING Check for errors
                    {
                        p.ErrorIndicator = true;
                        p.Level--;
                        return null;
                    }
                    goto done;
                }
                p.Mark = mark;
                PrintFail(ref p, "statement", "simple_stmts", mark);
            }

            result = null;
        done:
            p.Level--;
            return result;
        }

        // compound_stmt:
        //     | &('func' | '@' | ASYNC) function_def
        //     | &'if' if_stmt
        //     | &('class' | '@') class_def
        //     | &('with' | ASYNC) with_stmt
        //     | &('for' | ASYNC) for_stmt
        //     | &'try' try_stmt
        //     | &'while' while_stmt
        //     | match_stmt
        private static StmtType? RuleCompoundStmt(ref Parser p)
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
                return null;
            }
            StmtType? result = null;



            result = null;
        done:
            p.Level--;
            return result;
        }

        // simple_stmts: simple_stmt !';' NEWLINE | ';'.simple_stmt+ ';'? NEWLINE
        private static StmtSeq? RuleSimpleStmts(ref Parser p)
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
                return null;
            }
            StmtSeq? result = null;
            int mark = p.Mark;

            result = null;
        done:
            p.Level--;
            return result;
        }
    }
}
