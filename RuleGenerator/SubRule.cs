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
        public List<RuleObject> Objects;

        public SubRule(string format)
        {
            Format = format;
            Objects = new List<RuleObject>();

            Generate();
        }

        private void Generate()
        {
            string[] f = Format.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        private void AddObject(RuleObject rule)
        {
            Objects.Add(rule);
        }
    }
}
