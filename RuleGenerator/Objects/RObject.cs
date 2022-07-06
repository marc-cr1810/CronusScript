using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator.Objects
{
    [Flags]
    internal enum Conditions
    {
        Default = 0b000000,
        Loop = 0b000001,
        Loop_Opt = 0b000010,
        Not = 0b000100,
        Gather = 0b001000,
        Optional = 0b010000,
        Expected = 0b100000
    }

    internal class RObject
    {
        public readonly string Type;
        public string Format;
        public Conditions Conditions;

        public RObject(string type)
        {
            Type = type;
            Conditions = Conditions.Default;
        }

        public RObject(string type, string format)
        {
            Type = type;
            Format = format;
            Conditions = Conditions.Default;
        }

        public void SetConditions(Conditions conditions)
        {
            Conditions = conditions;
            FixFormat(Format);
        }

        private void FixFormat(string format)
        {
            if (Conditions.HasFlag(Conditions.Loop))
                format = $"{format}+";
            else if (Conditions.HasFlag(Conditions.Loop_Opt))
                format = $"{format}*";
            else if (Conditions.HasFlag(Conditions.Optional))
                format = $"{format}?";

            if (Conditions.HasFlag(Conditions.Not))
                format = $"!{format}";
            else if (Conditions.HasFlag(Conditions.Expected))
                format = $"&{format}";

            Format = format;
        }

        public static RObject[] GetObjectsFromFormat(string format)
        {
            List<RObject> objects = new List<RObject>();
            int parenLevel = 0;
            bool inQuote = false;

            string value = "";
            RObject sep = new RObject("null");
            RObject obj = new RObject("null");
            Conditions conditions = Conditions.Default;

            for (int i = 0; i < format.Length; i++)
            {
                char c = format[i];
                if (c == '\'' || c == '"')
                {
                    value = "";
                    char beginQuote = c;
                    c = format[++i];
                    while (c != beginQuote)
                    {
                        value += c;
                        if (i + 1 == format.Length)
                            break;
                        c = format[++i];
                    }
                    obj = TokenObject.FromQuote(value);
                    continue;
                }
                else if (c == '(' || c == '[')
                {
                    char endParen = (c == '(' ? ')' : ']');
                    int level = 0;
                    c = format[++i];
                    while (c != endParen || level != 0)
                    {
                        if (c == '(' || c == '[')
                            level++;
                        else if (c == ')' || c == ']')
                            level--;
                        value += c;
                        if (i + 1 == format.Length)
                            break;
                        c = format[++i];
                    }
                    if (endParen == ']')
                        obj = new GroupObject(value);
                    else
                        obj = new RuleGroupObject(value);
                    continue;
                }
                else if (IsPotentialIdentifierStart(c))
                {
                    value = "";
                    while (IsPotentialIdentifierChar(c))
                    {
                        value += c;
                        if (i + 1 == format.Length)
                            break;
                        c = format[++i];
                    }
                    obj = TokenObject.FromID(value);
                    if (obj.Type == "null")
                        obj = new RuleObject(value);
                }
                else if (c == '$')
                {
                    obj = new TokenObject("ENDMARKER", CronusScript.Parser.TokenType.ENDMARKER);
                    continue;
                }

                if (c == '+')
                    conditions |= Conditions.Loop;
                else if (c == '*')
                    conditions |= Conditions.Loop_Opt;
                else if (c == '!')
                    conditions |= Conditions.Not;
                else if (c == '?')
                    conditions |= Conditions.Optional;
                else if (c == '&')
                    conditions |= Conditions.Expected;
                else if (c == '.')
                {
                    conditions |= Conditions.Gather;
                    sep = obj;
                    obj = new RObject("null");
                    value = "";
                }

                if (c == ' ')
                {
                    if (obj.Type != "null")
                    {
                        if (conditions.HasFlag(Conditions.Gather))
                        {
                            GatherObject o = new GatherObject(sep, obj);
                            obj = o;
                        }
                        obj.SetConditions(conditions);
                        objects.Add(obj);
                    }
                    obj = new RObject("null");
                    conditions = Conditions.Default;
                    value = "";
                }
            }
            if (obj.Type != "null")
            {
                obj.SetConditions(conditions);
                objects.Add(obj);
            }

            return objects.ToArray();
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

        public override string ToString()
        {
            return $"{Type}: {Format}";
        }
    }
}
