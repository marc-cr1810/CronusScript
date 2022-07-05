using RuleGenerator.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator
{
    internal class SubRule
    {
        public readonly string Format;
        public List<RObject> Objects;

        public SubRule(string format)
        {
            Format = format;
            Objects = new List<RObject>();

            Generate();
        }

        private void Generate()
        {
            int parenLevel = 0;
            bool inQuote = false;

            string value = "";
            RObject obj = new RObject("null");
            Conditions conditions = Conditions.Default;
            for (int i = 0; i < Format.Length; i++)
            {
                char c = Format[i];
                if (c == '\'' || c == '"')
                {
                    value = "";
                    char beginQuote = c;
                    c = Format[++i];
                    while (c != beginQuote)
                    {
                        value += c;
                        if (i + 1 == Format.Length)
                            break;
                        c = Format[++i];
                    }
                    obj = TokenObject.FromQuote(value);
                    continue;
                }
                else if (c == '(' || c == '[')
                {
                    value = c.ToString();
                    char endParen = (c == '(' ? ')' : ']');
                    int level = 0;
                    c = Format[++i];
                    while (c != endParen || level != 0)
                    {
                        if (c == '(' || c == '[')
                            level++;
                        else if (c == ')' || c == ']')
                            level--;
                        value += c;
                        if (i + 1 == Format.Length)
                            break;
                        c = Format[++i];
                    }
                    value += c;
                    continue;
                }
                else if (IsPotentialIdentifierStart(c))
                {
                    value = "";
                    while(IsPotentialIdentifierChar(c))
                    {
                        value += c;
                        if (i + 1 == Format.Length)
                            break;
                        c = Format[++i];
                    }
                    obj = TokenObject.FromID(value);
                    if (obj.Type == "null")
                        obj = new RuleObject(value);
                }
                else if (c == '$')
                {
                    obj = new TokenObject("$", CronusScript.Parser.TokenType.ENDMARKER);
                    continue;
                }

                if (c == '+')
                    conditions |= Conditions.Loop;
                else if (c == '*')
                    conditions |= Conditions.Loop_Opt;
                else if (c == '!')
                    conditions |= Conditions.Not;

                if (c == ' ')
                {
                    obj.Conditions = conditions;
                    Objects.Add(obj);
                    obj = new RObject("null");
                    conditions = Conditions.Default;
                }
            }
            if (obj.Type != "null")
            {
                obj.Conditions = conditions;
                Objects.Add(obj);
            }
        }

        private void AddObject(RObject rule)
        {
            Objects.Add(rule);
        }

        private static bool IsPotentialIdentifierStart(int c)
        {
            return ((c >= 'a' && c <= 'z')
               || (c >= 'A' && c <= 'Z')
               || c == '_'
               || (c >= 128));
        }

        private static bool IsPotentialIdentifierChar(int c)
        {
            return ((c >= 'a' && c <= 'z')
               || (c >= 'A' && c <= 'Z')
               || (c >= '0' && c <= '9')
               || c == '_'
               || (c >= 128));
        }
    }
}
