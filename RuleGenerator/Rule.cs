using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleGenerator
{
    internal class Rule
    {
        public readonly string Name;
        public readonly string Format;
        public readonly string ResultType;
        public string Rules;

        public Rule(string name, string format, string resultType)
        {
            Name = name;
            Format = format;
            ResultType = resultType;
            Rules = "{}";

            Program.AddRule(this);
        }

        public void SetRules(string rules)
        {
            Rules = rules;
        }

        public override string ToString()
        {
            string result =
$@"// {Name}: {Format}
private static {ResultType} Rule{char.ToUpper(Name[0]) + Name.Substring(1)}(ref Parser p)
{{
    if (p.Level++ == MAXSTACK)
    {{
        p.ErrorIndicator = true;
        /// TODO: ERROR HANDLING
        Console.WriteLine(""No memory"");
    }}
    if (p.ErrorIndicator)
    {{
        p.Level--;
        return;
    }}
    
    {ResultType} result = null;
    int mark = p.Mark;
    
    
    
    result = null;
done:
    p.Level--;
    return result;
}}";
            return result;
        }
    }
}
