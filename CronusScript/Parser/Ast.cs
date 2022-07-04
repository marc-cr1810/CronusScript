using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CronusScript.Objects;

namespace CronusScript.Parser
{
    public enum BoolOpType
    {
        And = 1, Or = 2
    }

    public enum OperatorType
    {
        Add = 1, Sub = 2, Mult = 3, MatMult = 4, Div = 5, Mod = 6, Pow = 7,
        LShift = 8, RShift = 9, BitOr = 10, BitXor = 11, BitAnd = 12,
        FloorDiv = 13
    }

    public enum UnaryOpType
    {
        Invert = 1, Not = 2, UAdd = 3, USub = 4
    }

    public enum CmpOpType
    {
        Eq = 1, NotEq = 2, Lt = 3, LtE = 4, Gt = 5, GtE = 6, Is = 7, IsNot = 8,
        In = 9, NotIn = 10
    }

    internal struct Seq
    {
        public object[] Elements;
    }

    internal struct IdentifierSeq
    {
        public StringObject[] Identifiers;
    }

    internal struct IntSeq
    {
        public int[] Ints;
    }

    /* Module AST */

    public enum ModKind
    {
        Module = 1,
        Expression = 2,
        Function = 3
    }

    internal struct ModType
    {
        public struct _Module
        {
            public StmtSeq? Body;
            public TypeIgnoreSeq? TypeIgnores;
        }

        public struct _Expression
        {
            public ExprType Body;
        }

        public struct _FunctionType
        {
            public ExprSeq ArgTypes;
            public ExprType Returns;
        }

        public ModKind Kind;
        public _Module Module;
        public _Expression Expression;
        public _FunctionType FunctionType;
    }

    internal struct ModSeq
    {
        public ModType[] Mods;
    }

    /* Statement AST */

    public enum StmtKind
    {
        FunctionDef = 1, AsyncFunctionDef = 2, ClassDef = 3,
        Return = 4, Delete = 5, Assign = 6,
        AugAssign = 7, AnnAssign = 8, For = 9,
        AsyncFor = 10, While = 11, If = 12, With = 13,
        AsyncWith = 14, Match = 15, Raise = 16, Try = 17,
        TryStar = 18, Assert = 19, Import = 20,
        ImportFrom = 21, Global = 22, Nonlocal = 23,
        Expr = 24, Pass = 25, Break = 26, Continue = 27
    }

    internal struct StmtType
    {
        public struct _FunctionDef
        {
            public StringObject Name;
            public ArgumentsType Args;
            public StmtSeq Body;
            public ExprSeq DecoratorList;
            public ExprType Returns;
            public StringObject TypeComment;
        }

        public struct _AsyncFunctionDef
        {
            public StringObject Name;
            public ArgumentsType Args;
            public StmtSeq Body;
            public ExprSeq DecoratorList;
            public ExprType Returns;
            public StringObject TypeComment;
        }

        public struct _ClassDef
        {
            public StringObject Name;
            public ExprSeq Bases;
            public KeywordSeq Keywords;
            public StmtSeq Body;
            public ExprSeq DecoratorList;
        }

        public struct _Return
        {
            public ExprType Value;
        }

        public struct _Delete
        {
            public ExprSeq Targets;
        }

        public struct _Assign
        {
            public ExprSeq Targets;
            public ExprType Value;
            public StringObject TypeComment;
        }

        public struct _AugAssign
        {
            public ExprType Target;
            public OperatorType Op;
            public StringObject TypeComment;
        }

        public struct _AnnAssign
        {
            public ExprType Target;
            public ExprType Annotation;
            public ExprType Value;
            int Simple;
        }

        public struct _For
        {
            public ExprType Target;
            public ExprType Iter;
            public StmtSeq Body;
            public StmtSeq OrElse;
            public StringObject TypeComment;
        }

        public struct _AsyncFor
        {
            public ExprType Target;
            public ExprType Iter;
            public StmtSeq Body;
            public StmtSeq OrElse;
            public StringObject TypeComment;
        }

        public struct _While
        {
            public ExprType Test;
            public StmtSeq Body;
            public StmtSeq OrElse;
        }

        public struct _If
        {
            public ExprType Test;
            public StmtSeq Body;
            public StmtSeq OrElse;
        }

        public struct _With
        {
            public WithItemSeq Items;
            public StmtSeq Body;
            public StringObject TypeComment;
        }

        public struct _AsyncWith
        {
            public WithItemSeq Items;
            public StmtSeq Body;
            public StringObject TypeComment;
        }

        public struct _Raise
        {
            public ExprType Exc;
            public ExprType Cause;
        }

        public struct _Try
        {
            public StmtSeq Body;
            public ExceptHandlerSeq Handlers;
            public StmtSeq OrElse;
            public StmtSeq FinalBody;
        }

        public struct _TryStar
        {
            public StmtSeq Body;
            public ExceptHandlerSeq Handlers;
            public StmtSeq OrElse;
            public StmtSeq FinalBody;
        }

        public struct _Assert
        {
            public ExprType Test;
            public ExprType Msg;
        }

        public struct _Import
        {
            public AliasSeq Names;
        }

        public struct _ImportFrom
        {
            public StringObject Module;
            public AliasSeq Names;
            public int Level;
        }

        public struct _Global
        {
            public IdentifierSeq Names;
        }

        public struct _NonLocal
        {
            public IdentifierSeq Names;
        }

        public struct _Expr
        {
            public ExprType Expr;
        }

        public StmtKind Kind;

        public _FunctionDef FunctionDef;
        public _AsyncFunctionDef AsyncFunctionDef;
        public _ClassDef ClassDef;
        public _Return Return;
        public _Delete Delete;
        public _Assign Assign;
        public _AugAssign AugAssign;
        public _AnnAssign AnnAssign;
        public _For For;
        public _While While;
        public _If If;
        public _With With;
        public _AsyncWith AsyncWith;
        public _Raise Raise;
        public _Try Try;
        public _TryStar TryStar;
        public _Assert Assert;
        public _Import Import;
        public _ImportFrom ImportFrom;
        public _Global Global;
        public _NonLocal NonLocal;
        public _Expr Expr;

        public int LineNo;
        public int ColOffset;
        public int EndLineNo;
        public int EndColOffset;

        public StmtSeq SingletonSeq()
        {
            StmtSeq seq = new StmtSeq();
            seq.Stmts = new StmtType[] { this };
            return seq;
        }
    }

    internal struct StmtSeq
    {
        public StmtType[] Stmts;
    }

    internal struct StmtSeqSeq
    {
        public StmtSeq?[] Elements;

        public StmtSeq Flatten()
        {
            List<StmtType> types = new List<StmtType>();
            foreach (StmtSeq seq in Elements)
            {
                foreach (StmtType type in seq.Stmts)
                {
                    types.Add(type);
                }
            }
            StmtSeq result = new StmtSeq();
            result.Stmts = types.ToArray();
            return result;
        }
    }

    /* Expression AST */

    public enum ExprContext
    {
        Load = 1, Store = 2, Del = 3
    }

    public enum ExprKind
    {
        BoolOp = 1, NamedExpr = 2, BinOp = 3, UnaryOp = 4,
        Lambda = 5, IfExp = 6, Dict = 7, Set = 8,
        ListComp = 9, SetComp = 10, DictComp = 11,
        GeneratorExp = 12, Await = 13, Yield = 14,
        YieldFrom = 15, Compare = 16, Call = 17,
        FormattedValue = 18, JoinedStr = 19, Constant = 20,
        Attribute = 21, Subscript = 22, Starred = 23,
        Name = 24, List = 25, Tuple = 26, Slice = 27
    }

    internal class ExprType
    {
        public struct _BoolOp
        {
            public BoolOpType op;
            public ExprSeq Values;
        }

        public struct _NamedExpr
        {
            public ExprType Target;
            public ExprType Value;
        }

        public struct _BinOp
        {
            public ExprType Left;
            public OperatorType Op;
            public ExprType Right;
        }

        public struct _UnaryOp
        {
            public UnaryOpType Op;
            public ExprType Operand;
        }

        public struct _Lambda
        {
            public ArgumentsType Args;
            public ExprType Body;
        }

        public struct _IfExp
        {
            public ExprType Test;
            public ExprType Body;
            public ExprType OrElse;
        }

        public struct _Dict
        {
            public ExprSeq Keys;
            public ExprSeq Values;
        }

        public struct _Set
        {
            public ExprSeq Elts;
        }

        public struct _ListComp
        {
            public ExprType Elt;
            public ComprehensionSeq Generators;
        }

        public struct _SetComp
        {
            public ExprType Elt;
            public ComprehensionSeq Generators;
        }

        public struct _DictComp
        {
            public ExprType Key;
            public ExprType Value;
            public ComprehensionSeq Generators;
        }

        public struct _GeneratorExp
        {
            public ExprType Elt;
            public ComprehensionSeq Generators;
        }

        public struct _Await
        {
            public ExprType Value;
        }

        public struct _Yield
        {
            public ExprType Value;
        }

        public struct _YieldFrom
        {
            public ExprType Value;
        }

        public struct _Compare
        {
            public ExprType Left;
            public IntSeq Ops;
            public ExprSeq Comparators;
        }

        public struct _Call
        {
            public ExprType Func;
            public ExprSeq Args;
            public KeywordSeq Keywords;
        }

        public struct _FormattedValue
        {
            public ExprType Value;
            public int Converstion;
            public ExprType FormatSpec;
        }

        public struct _JoinedStr
        {
            public ExprSeq Values;
        }

        public struct _Constant
        {
            CObject Value;
            StringObject Kind;
        }

        public struct _Attribute
        {
            public ExprType Value;
            public StringObject Attr;
            public ExprContext Ctx;
        }

        public struct _Subscript
        {
            public ExprType Value;
            public ExprType Slice;
            public ExprContext Ctx;
        }

        public struct _Starred
        {
            public ExprType Value;
            public ExprContext Ctx;
        }

        public struct _Name
        {
            StringObject Id;
            ExprContext Ctx;
        }

        public struct _List
        {
            public ExprSeq Elts;
            public ExprContext Ctx;
        }

        public struct _Tuple
        {
            public ExprSeq Elts;
            public ExprContext Ctx;
        }

        public struct _Slice
        {
            public ExprType Lower;
            public ExprType Upper;
            public ExprType Step;
        }

        public ExprKind Kind;

        public _BoolOp BoolOp;
        public _NamedExpr NamedExpr;
        public _BinOp BinOp;
        public _UnaryOp UnaryOp;
        public _Lambda Lambda;
        public _IfExp IfExp;
        public _Dict Dict;
        public _Set Set;
        public _ListComp ListComp;
        public _SetComp SetComp;
        public _DictComp DictComp;
        public _GeneratorExp GeneratorExp;
        public _Await Await;
        public _Yield Yield;
        public _YieldFrom YieldFrom;
        public _Compare Compare;
        public _Call Call;
        public _FormattedValue FormattedValue;
        public _JoinedStr JoinedStr;
        public _Constant Constant;
        public _Attribute Attribute;
        public _Subscript Subscript;
        public _Starred Starred;
        public _Name Name;
        public _List List;
        public _Tuple Tuple;
        public _Slice Slice;

        public int LineNo;
        public int ColOffset;
        public int EndLineNo;
        public int EndColOffset;
    }

    internal struct ExprSeq
    {
        public ExprType[] Exprs;
    }

    /* Comprehension AST */

    internal struct ComprehensionType
    {
        public ExprType Target;
        public ExprType Iter;
        public ExprSeq Ifs;
        public bool IsAsync;
    }

    internal struct ComprehensionSeq
    {
        public ComprehensionType[] Comprehensions;
    }

    /* Except Handler AST */

    public enum ExceptHandlerKind
    {
        ExceptHandler = 1
    }

    internal struct ExceptHandlerType
    {
        public struct _ExceptHandler
        {
            public ExprType Type;
            StringObject Name;
            StmtSeq Body;
        }

        public ExceptHandlerKind Kind;
        public _ExceptHandler ExceptHandler;
        public int LineNo;
        public int ColOffset;
        public int EndLineNo;
        public int EndColOffset;
    }

    internal struct ExceptHandlerSeq
    {
        public ExceptHandlerType[] ExceptHandlers;
    }

    /* Arguments AST */

    internal struct ArgumentsType
    {
        public ArgSeq PosOnlyArgs;
        public ArgSeq Args;
        public ArgType VarArg;
        public ArgSeq KWOnlyArgs;
        public ExprSeq KWDefaults;
        public ArgType KWArg;
        public ExprSeq Defaults;
    }

    /* Arg AST */

    internal struct ArgType
    {
        public StringObject Arg;
        public ExprType Annotation;
        public StringObject TypeComment;
        public int LineNo;
        public int ColOffset;
        public int EndLineNo;
        public int EndColOffset;
    }

    internal struct ArgSeq
    {
        public ArgType Args;
    }

    /* Keyword AST */

    internal struct KeywordType
    {
        public StringObject Arg;
        public ExprType Value;
        public int LineNo;
        public int ColOffset;
        public int EndLineNo;
        public int EndColOffset;
    }

    internal struct KeywordSeq
    {
        public KeywordType[] Keywords;
    }

    /* Alias AST */

    internal struct AliasType
    {
        StringObject Name;
        StringObject AsName;
        public int LineNo;
        public int ColOffset;
        public int EndLineNo;
        public int EndColOffset;
    }

    internal struct AliasSeq
    {
        public AliasType[] Aliases;
    }

    /* With Item AST */

    internal struct WithItemType
    {
        public ExprType ContextExpr;
        public ExprType OptionalVars;
    }

    internal struct WithItemSeq
    {
        public WithItemType[] WithItems;
    }

    /* TypeIgnore AST */

    public enum TypeIgnoreKind { TypeIgnore = 1 }

    internal struct TypeIgnoreType
    {
        public struct _TypeIgnore
        {
            public int LineNo;
            public StringObject Tag;
        }

        public TypeIgnoreKind Kind;
        public _TypeIgnore TypeIgnore;
    }

    internal struct TypeIgnoreSeq
    {
        public TypeIgnoreType[] TypeIgnores;
    }

    /*
     *      AST Node Generator Functions
     */

    internal static class AST
    {
        internal static ModType Module(StmtSeq? body, TypeIgnoreSeq? typeIgnores)
        {
            ModType p = new ModType();
            p.Kind = ModKind.Module;
            p.Module.Body = body;
            p.Module.TypeIgnores = typeIgnores;
            return p;
        }
    }
}
