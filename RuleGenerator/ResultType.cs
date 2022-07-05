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
        ModType = 1, ModSeq = 2, 
        StmtType = 3, StmtSeq = 4, StmtSeqSeq = 5,
        ExprType = 6, ExprSeq = 7,
        ComprehensionType = 8, ComprehensionSeq = 9,
        ExceptHandlerType = 10, ExceptHandlerSeq = 11,
        ArgumentsType = 12,
        ArgType = 13, ArgSeq = 14,
        KeywordType = 15, KeywordSeq = 16,
        AliasType = 17, AliasSeq = 18,
        WithItemType = 19, WithItemSeq = 20,
        TypeIgnoreType = 21, TypeIgnoreSeq = 22
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
