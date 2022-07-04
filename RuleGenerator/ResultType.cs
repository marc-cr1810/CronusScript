using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CronusScript.Parser;

namespace RuleGenerator
{
    internal enum NodeType
    {
        Mod = 1, ModSeq = 2, 
        Stmt = 3, StmtSeq = 4, StmtSeqSeq = 5,
        Expr = 6, ExprSeq = 7,
        Comprehension = 8, ComprehensionSeq = 9,
        ExceptHandler = 10, ExceptHandlerSeq = 11,
        Arguments = 12,
        Arg = 13, ArgSeq = 14,
        Keyword = 15, KeywordSeq = 16,
        Alias = 17, AliasSeq = 18,
        WithItem = 19, WithItemSeq = 20,
        TypeIgnore = 21, TypeIgnoreSeq = 22
    }

    internal struct NodeTypeKind
    {
        public ModKind ModKind;
        public StmtKind StmtKind;
        public ExprKind ExprKind;
        public ExceptHandlerKind ExceptHandlerKind;
        public TypeIgnoreKind TypeIgnoreKind;
    }

    internal struct ResultType
    {
        public NodeType Type;
        public NodeTypeKind? Kind;
    }
}
