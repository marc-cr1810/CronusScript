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

        public string GetKindString()
        {
            if (Kind == null)
                return "";
            switch (Type)
            {
                case NodeType.ModType:
                    return Kind.Value.ModKind.ToString();
                case NodeType.StmtType:
                    return Kind.Value.StmtKind.ToString();
                case NodeType.ExprType:
                    return Kind.Value.ExprKind.ToString();
                case NodeType.ExceptHandlerType:
                    return Kind.Value.ExceptHandlerKind.ToString();
                case NodeType.TypeIgnoreType:
                    return Kind.Value.TypeIgnoreKind.ToString();
            }
            return "";
        }

        public ResultType ToSeq()
        {
            ResultType result = new ResultType();

            switch (Type)
            {
                case NodeType.ModType:
                    result.Type = NodeType.ModSeq;
                    break;
                case NodeType.StmtType:
                    result.Type = NodeType.StmtSeq;
                    break;
                case NodeType.StmtSeq:
                    result.Type = NodeType.StmtSeqSeq;
                    break;
                case NodeType.ExprType:
                    result.Type = NodeType.ExprSeq;
                    break;
                case NodeType.ComprehensionType:
                    result.Type = NodeType.ComprehensionSeq;
                    break;
                case NodeType.ExceptHandlerType:
                    result.Type = NodeType.ExceptHandlerSeq;
                    break;
                case NodeType.ArgType:
                    result.Type = NodeType.ArgSeq;
                    break;
                case NodeType.KeywordType:
                    result.Type = NodeType.KeywordSeq;
                    break;
                case NodeType.AliasType:
                    result.Type = NodeType.AliasSeq;
                    break;
                case NodeType.WithItemType:
                    result.Type = NodeType.WithItemSeq;
                    break;
                case NodeType.TypeIgnoreType:
                    result.Type = NodeType.TypeIgnoreSeq;
                    break;
            }

            return result;
        }

        public static NodeTypeKind GetKind(NodeType? type, string s)
        {
            NodeTypeKind kind = new NodeTypeKind();
            if (s == "No kind")
                return kind;

            switch (type)
            {
                case NodeType.ModType:
                    {
                        kind.ModKind = (ModKind)Enum.Parse(typeof(ModKind), s);
                    }
                    break;
                case NodeType.StmtType:
                    {
                        kind.StmtKind = (StmtKind)Enum.Parse(typeof(StmtKind), s);
                    }
                    break;
                case NodeType.ExprType:
                    {
                        kind.ExprKind = (ExprKind)Enum.Parse(typeof(ExprKind), s);
                    }
                    break;
                case NodeType.ExceptHandlerType:
                    {
                        kind.ExceptHandlerKind = (ExceptHandlerKind)Enum.Parse(typeof(ExceptHandlerKind), s);
                    }
                    break;
                case NodeType.TypeIgnoreType:
                    {
                        kind.TypeIgnoreKind = (TypeIgnoreKind)Enum.Parse(typeof(TypeIgnoreKind), s);
                    }
                    break;
            }
            return kind;
        }
    }
}
