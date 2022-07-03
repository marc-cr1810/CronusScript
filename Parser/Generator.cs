using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CronusScript.Objects;

namespace CronusScript.Parser
{
    internal class Generator
    {
        public static void RunParser(StringObject filepath)
        {
            TokState tok = new TokState(filepath);

            Parser p = new Parser(tok);

            p.Parse();
        }
    }
}
